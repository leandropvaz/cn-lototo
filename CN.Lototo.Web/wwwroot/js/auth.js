window.lototoAuth = {
    login: async function (loginDto, returnUrl) {
        try {
            const response = await fetch('/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(loginDto),
                credentials: 'include' // ESSENCIAL para enviar/receber cookie
            });

            console.log('LOGIN /auth/login status:', response.status);

            // NÃO chamar response.json() aqui – a API devolve só 200/401 sem body JSON
            if (!response.ok) {
                // Se deu 401, 400, 500, etc.
                return false;
            }

            const destino = returnUrl && returnUrl.length > 0 ? returnUrl : '/home';

            // Redireciona o BROWSER (já com cookie, se foi criado)
            window.location.href = destino;
            return true;
        } catch (e) {
            console.error('Erro no login:', e);
            return false;
        }
    },

    logout: async function () {
        try {
            const response = await fetch('/auth/logout', {
                method: 'POST',
                credentials: 'include'
            });

            console.log('LOGOUT /auth/logout status:', response.status);

            window.location.replace('/');
        } catch (e) {
            console.error('Erro no logout:', e);
        }
    }
};
