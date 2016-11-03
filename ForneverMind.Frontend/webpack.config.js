const ExtractTextPlugin = require("extract-text-webpack-plugin");
const path = require('path');

const outputPath = path.join(__dirname, '..', 'ForneverMind', 'app');

module.exports = {
    resolve: { extensions: ['', '.js', '.less'] },
    entry: { 'app': './app' },
    module: {
        loaders: [{ test: /\.less$/, loader: ExtractTextPlugin.extract("style-loader", "css-loader!less-loader") }]
    },
    plugins: [new ExtractTextPlugin('[name].css')],
    output: { path: outputPath, filename: '[name].js' }
};
