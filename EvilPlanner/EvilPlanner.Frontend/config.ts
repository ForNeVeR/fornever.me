// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

type Language = 'en' | 'ru';
function getLocalization(language: Language) {
    switch (language) {
        case 'ru': return {
            fetchingQuote: 'Получение цитаты…',
            quoteOfTheDay: 'Цитата дня'
        };
        default: return {
            fetchingQuote: 'Fetching quote…',
            quoteOfTheDay: 'Quote of the Day'
        };
    }
}

export = {
    // Converts a link like https://fornever.me/plans/en/index.html to https://fornever.me/plans
    api: window.location.pathname.split('/').slice(0, -2).join('/'),
    language: window.location.pathname.indexOf('/ru/') != -1 ? 'ru' : 'en',
    get localization() {
        return getLocalization(this.language)
    }
};
