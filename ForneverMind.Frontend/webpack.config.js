const ExtractTextPlugin = require('extract-text-webpack-plugin');
const path = require('path');

const outputPath = path.join(__dirname, '..', 'ForneverMind', 'wwwroot', 'app');

const client = {
    resolve: { extensions: ['', '.js', '.less'] },
    entry: { 'app': './app' },
    module: {
        loaders: [{ test: /\.less$/, loader: ExtractTextPlugin.extract('style-loader', 'css-loader!less-loader') }]
    },
    plugins: [new ExtractTextPlugin('[name].css')],
    output: { path: outputPath, filename: '[name].js' }
};

const server = {
    resolve: { extensions: ['', '.js'] },
    entry: './server',
    output: { path: outputPath, filename: 'server.js', libraryTarget: 'umd' }
};

module.exports = [client, server];
