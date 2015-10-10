/// <reference path="typings/tsd.d.ts"/>

declare var require: any;
import React = require('react');
var ReactDOM = require('react-dom'); // TODO: Get rid of ReactDOM type errors

import QuotationBlock = require('./quotation-block');

ReactDOM.render(
    <QuotationBlock />,
    document.getElementById('content'));
