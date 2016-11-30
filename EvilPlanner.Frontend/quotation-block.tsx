import React = require('react');
import config = require('./config');
import 'whatwg-fetch';

function padWithZeros(text: string, length: number): string {
    while (text.length < length) {
        text = '0' + text;
    }

    return text;
}

function formatDate(date: Date): string {
    return `${date.getUTCFullYear()}-${padWithZeros((date.getUTCMonth() + 1).toString(), 2)}-${padWithZeros(date.getUTCDate().toString(), 2)}`
}

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
            <a className="source-link" href={this.state.sourceUrl}>{this.state.source}</a>
        </div>;
    }

    componentDidMount() {
        var param = formatDate(new Date());
        window.fetch(`${config.api}/quote/${param}`).then((response) => {
            return response.json();
        }).then((message) => {
            this.setState(message);
        });
    }
}

export = QuotationBlock;
