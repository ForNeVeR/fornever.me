import React = require('react');
import ReactDOM = require('react-dom');

import QuotationBlock = require('./quotation-block.tsx');

declare var require: (string) => void;
require('./index');
require('./css/main');
require('./css/quotations');

ReactDOM.render(
    <QuotationBlock />,
    document.getElementById('content'));
