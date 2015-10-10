/// <reference path="typings/tsd.d.ts"/>

import React = require('react');
import config = require('./config');

declare var require: any;
require('whatwg-fetch');

interface QuotationBlockState {
    text: string;
    sourceUrl: string;
    source: string;
}

class QuotationBlock extends React.Component<{}, QuotationBlockState>{
    constructor() {
        super();
        this.state = {
            text: 'Fetching quote...',
            source: '',
            sourceUrl: '#'
        };
    }

    render() {
        return <div>
            <h1>Quote of the Day</h1>

            <blockquote>{this.state.text}</blockquote>
            <a classID="source-link" href="{this.state.sourceUrl}">{this.state.source}</a>
        </div>;
    }

    componentDidMount() {
        var date = new Date();
        var param = `${date.getUTCFullYear()}-${date.getUTCMonth() + 1}-${date.getUTCDate()}`;
        window.fetch(`${config.api}/quote/${param}`).then((response) => {
            return response.json();
        }).then((message) => {
            this.setState(message);
        });
    }
}

export = QuotationBlock;
