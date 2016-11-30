const path = require('path');

const outputPath = path.join(__dirname, 'dist');

module.exports = {
    resolve: { extensions: ['', '.css', '.js', '.html', '.ts', '.tsx'] },
    entry: { 'app': './app' },
    module: {
        loaders: [
            { test: /\.html$/, loader: 'file?name=[name].[ext]' },
            { test: /\.css$/, loader: 'file?name=[name].[ext]' },
            { test: /\.ts(x?)$/, loader: 'ts' }
        ]
    },
    output: { path: outputPath, filename: '[name].js' }
};
