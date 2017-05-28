const hljs = require('highlight.js');
module.exports = (callback, language, text) => {
    const result = language && hljs.getLanguage(language)
        ? hljs.highlight(language, text).value
        : hljs.highlightAuto(text).value;
    callback(null, result);
};
