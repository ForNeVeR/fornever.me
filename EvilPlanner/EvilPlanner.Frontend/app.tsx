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
