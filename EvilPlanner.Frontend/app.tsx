/// <reference path="typings/tsd.d.ts"/>

import React = require('react');
import ReactDOM = require('react-dom');

var HelloMessage = React.createClass({
  render: function() {
    return <div>Hello World</div>;
  }
});

React.render(<HelloMessage />, document.getElementById('content'));
