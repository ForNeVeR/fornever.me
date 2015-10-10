/// <reference path="typings/tsd.d.ts"/>

import React = require('react');
import ReactDOM = require('react-dom');

import config = require('./config');

declare var require: any;
require('whatwg-fetch');

var HelloMessage = React.createClass({
  render: function() {
    return <div>Hello World</div>;
  }
});

React.render(<HelloMessage />, document.getElementById('content'));

window.fetch(`${config.api}/quote/2015-10-10`).then((data) => {
    window.alert(data);
})
