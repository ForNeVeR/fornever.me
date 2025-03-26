// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

const hljs = require('highlight.js');
module.exports = function (language, text) {
    return language && hljs.getLanguage(language)
        ? hljs.highlight(language, text).value
        : hljs.highlightAuto(text).value;
};
