const path = require('path');

const outputPath = path.join(__dirname, '../EvilPlanner.Backend/wwwroot');

module.exports = {
    resolve: { extensions: ['.css', '.js', '.html', '.ts', '.tsx'] },
    entry: { 'app': './app' },
    module: {
        rules: [
            { test: /\.html$/, loader: 'file-loader?name=[path][name].[ext]' },
            { test: /\.css$/, loader: 'file-loader?name=[name].[ext]' },
            { test: /\.ts(x?)$/, loader: 'ts-loader' }
        ]
    },
    output: { path: outputPath, filename: '[name].js' }
};
