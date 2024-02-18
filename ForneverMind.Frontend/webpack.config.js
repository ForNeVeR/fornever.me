const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const path = require('path');

const outputPath = path.join(__dirname, '..', 'ForneverMind');

const client = {
    resolve: {extensions: ['', '.js', '.less']},
    entry: {'app': './style/main'},
    module: {
        rules: [{
            test: /\.less$/i,
            use: [
                MiniCssExtractPlugin.loader,
                'css-loader'
            ]
        }]
    },
    plugins: [new MiniCssExtractPlugin({filename: '[name].css'})],
    output: {path: path.join(outputPath, 'wwwroot', 'app'), filename: '[name].js'}
};

const server = {
    resolve: {extensions: ['', '.js']},
    entry: './server',
    output: {path: outputPath, filename: 'server.js', libraryTarget: 'var', library: 'server'},
    performance: {
        hints: false
    }
};

module.exports = [client, server];
