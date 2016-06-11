const path = require('path');

module.exports = {
    resolve: {
        extensions: ['', '.js']
    },

    output: {
        path: path.join(__dirname, '..', 'ForneverMind', 'app'),
        filename: '[name].js'
    },

    entry: {
        'app': 'microlight'
    }
};
