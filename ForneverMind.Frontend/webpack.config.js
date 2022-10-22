const ExtractTextPlugin = require('extract-text-webpack-plugin');
const path = require('path');

const outputPath = path.join(__dirname, '..', 'ForneverMind');

const client = {
    resolve: { extensions: ['', '.js', '.less'] },
    entry: { 'app': './style/main' },
    module: {
        loaders: [{ test: /\.less$/, loader: ExtractTextPlugin.extract('style-loader', 'css-loader!less-loader') }]
    },
    plugins: [new ExtractTextPlugin('[name].css')],
    output: { path: path.join(outputPath, 'wwwroot', 'app'), filename: '[name].js' }
};

const server = {
    resolve: { extensions: ['', '.js'] },
    entry: './server',
    output: { path: outputPath, filename: 'server.js', libraryTarget: 'var', library: 'server' }
};

module.exports = [client, server];
