// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

import React = require('react');
import ReactDOM = require('react-dom');

import QuotationBlock = require('./quotation-block');

declare var require: (string) => void;
require('./en/index');
require('./ru/index');
require('./css/main');
require('./css/quotations');

ReactDOM.render(
    <QuotationBlock />,
    document.getElementById('content'));
