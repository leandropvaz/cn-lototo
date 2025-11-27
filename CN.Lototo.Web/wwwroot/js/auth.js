window.lototoLogin = async function (loginDto) {
    const response = await fetch('/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        credentials: 'include', // MUITO IMPORTANTE: inclui cookies
        body: JSON.stringify(loginDto)
    });

    return response.ok;
};
