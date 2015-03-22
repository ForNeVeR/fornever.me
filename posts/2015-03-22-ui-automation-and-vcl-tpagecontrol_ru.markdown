---
title: UI Automation и TPageControl из библиотеки VCL
description: Работа со страницами VCL TPageControl с помощью .NET UI Automation.
---

[UI Automation](https://msdn.microsoft.com/en-us/library/ms753107(v=vs.110\).aspx) - это механизм, который может быть
использован для разных целей. Мне он сегодня пригодился для взаимодействия с одним полезным устройством, производитель
которого, к сожалению, не предоставил никакого интерфейса для взаимодействия с ним, кроме графического интерфейса под
Windows. Что ж, случается всякое, и такие сценарии тоже иногда хочется автоматизировать.

Программа написана с использованием технологий Borland (да, она достаточно старая), для графического интерфейса
используется старая добрая библиотека VCL.

Есть хороший [пост на StackOverflow](http://stackoverflow.com/a/22641792/2684760), который на простом примере поясняет
работу с UI Automation. Я попытался следовать ему при реализации своего кода, но столкнулся с некоторыми проблемами, в
частности, при работе с компонентом `TPageControl`. В результате каких-то непонятных проблем вкладки внутри этого
контрола неправильно определяются средствами UI Automation. Этот пост посвящён тому, как можно малой кровью обойти эти
ограничения.

Сначала посмотрим на окно программы с помощью [Spy++](https://msdn.microsoft.com/en-us/library/vstudio/dd460760.aspx).

<img src="/images/2015-03-22-spyxx-tree.png"/>

Вроде как по скриншоту видно, что у `TPageControl` есть дочернее окно класса `TTabSheet` (напоминаю, что в WinAPI все
элементы управления называются "окнами"). Ну что ж, попробуем получить эти дочерние элементы:

```cs
var desktop = AutomationElement.RootElement;
var window = desktop.FindFirst(
	TreeScope.Children,
	new PropertyCondition(AutomationElement.NameProperty, "█████████████████████████"));
var pageControl = window.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "TPageControl"));
var settingsTab = pageControl.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "TTabSheet"));
```

Пока всё работает - это код выполняется и возвращает `settingsTab`. Однако у этого объекта свойство
`Current.NativeWindowHandle` равняется `0`, что не соответствует действительности. Дальнейшие вызовы методов
`settingsTab.FindFirst` и `settingsTab.FindAll` ни к чему не приведут (вернут, соответственно, `null` или пустую
коллекцию элементов). Точно так же функционирует "ручной" перебор элементов с помощью более низкоуровневого класса
`TreeWalker`.

В чём конкретно проблема именно с этим классом - непонятно, однако есть намёки на то, как её решить. Поскольку Spy++
корректно показывает дерево окон - значит, с точки зрения ОС всё в порядке, а проблема только в UI Automation. Поэтому
мы попробуем получить дескрипторы этих окон и построить с их помощью новые, неиспорченные экземпляры
`AutomationElement`.

К сожалению, для этого придётся поиграться немного с P/Invoke, но это нас не остановит. Итак, приступаем. Напишем
функцию, которая будет получать _правильный_ `AutomationElement` для дочерней вкладки элемента управления
`TPageControl`:

```cs
public static AutomationElement GetPageControlTab(AutomationElement pageControl, string name)
{
	var handle = (IntPtr)pageControl.Current.NativeWindowHandle;
	var results = new List<IntPtr>();
	NativeHelper.EnumWindowsProc callback = (hwnd, lparam) =>
	{
		var className = NativeHelper.GetWindowClassName(hwnd);
		var tabName = NativeHelper.GetWindowText(hwnd);
		if (className == "TTabSheet" && tabName == name)
		{
			results.Add(hwnd);
		}

		return false;
	};

	NativeHelper.EnumChildWindows(handle, callback, IntPtr.Zero);
	GC.KeepAlive(callback);

	var tabHandle = results.Single();
	return AutomationElement.FromHandle(tabHandle);
}
```

Здесь `NativeHelper` - это вот такой вспомогательный класс:

```cs
internal class NativeHelper
{
	public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

	[DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern int GetWindowText(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nMaxCount);

	public static string GetWindowClassName(IntPtr hwnd)
	{
		var buffer = new StringBuilder(128);
		GetClassName(hwnd, buffer, buffer.Capacity);
		return buffer.ToString();
	}

	public static string GetWindowText(IntPtr hwnd)
	{
		var buffer = new StringBuilder(128);
		GetWindowText(hwnd, buffer, buffer.Capacity);
		return buffer.ToString();
	}
}
```

И теперь перепишем код с использованием этой функции:

```cs
var desktop = AutomationElement.RootElement;
var window = desktop.FindFirst(
	TreeScope.Children,
	new PropertyCondition(AutomationElement.NameProperty, "█████████████████████████"));
var pageControl = window.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "TPageControl"));
var settingsTab = AutomationHelper.GetPageControlTab(pageControl, "Установки");
```

Полученный таким образом объект `settingsTab` уже можно обрабатывать стандартными методами - у него нормальное состояние
свойства `NativeWindowHandle`, поэтому все методы `FindFirst`, `FindAll` и т.п. работают с ним корректно.

Я полагаю, что этот способ может быть актуальным не только для работы с VCL-компонентами, но и с другими проблемными
оконными библиотеками, если такие обнаружатся.
