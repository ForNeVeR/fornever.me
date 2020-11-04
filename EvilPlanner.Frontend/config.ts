export = {
    // Converts a link like https://fornever.me/plans/en/index.html to https://fornever.me/plans
    api: window.location.pathname.split('/').slice(0, -2).join('/')
};
