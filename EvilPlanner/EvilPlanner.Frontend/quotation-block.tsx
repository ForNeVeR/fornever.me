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
    constructor(props) {
        super(props);
        this.state = {
            text: config.localization.fetchingQuote,
            source: '',
            sourceUrl: '#'
        };
    }

    render() {
        const otherLanguage = config.language == 'en' ? 'ru' : 'en';
        const otherLanguageDisplayName = config.language == 'en' ? 'Rus' : 'Eng';
        const otherLanguageLink = `../${otherLanguage}/index.html`;
        return <div>
            <div className="header-group">
                <h1 className="header">{config.localization.quoteOfTheDay}</h1>
                <div className="languages">
                    <a href={otherLanguageLink} className="tag">{otherLanguageDisplayName}</a>
                </div>
            </div>

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
