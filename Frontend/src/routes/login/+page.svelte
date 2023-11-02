<script>
	import { accessToken, isAdminStore, usernameStore } from "../../stores";

    let username = '';
    let password = '';
    let loginResult = '';
    let loginSuccessful = false;
    let isAdmin = false;
    /**
	 * @type {string}
	 */
    let accessTokenValue;
    
    accessToken.subscribe((value => {
        accessTokenValue = value;
    }))

    $: loginStyle = loginSuccessful ? "variant-ringed-success" : "variant-ringed-error";

    /**
	 * @param {any} username
	 * @param {any} password
	 */
    function login(username, password) {
        const data = {
            username: username,
            password: password
        }
        fetch('http://localhost:5157/api/v1/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
        .then(async response => {
            console.log(response);
            if(!response.ok) {
                loginResult = await response.text();
                console.log(loginResult);
                loginSuccessful = false;
                throw loginResult;
            } else {
                return response.json();
            }
        })
        .then(response => {
            loginResult = 'User logged in successfully!';
            accessToken.set(response.accessToken);
            usernameStore.set(username);
            checkAdminRole();
            isAdminStore.set(isAdmin);
            console.log("is admin: " + isAdmin);
            loginSuccessful = true;
            console.log(response);
        })
        .catch((error) => console.error('Error logging in', error));
    }

    function checkAdminRole() {
        try {
            const decodedToken = parseJwt(accessTokenValue);
            let roles = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
            console.log(roles);

            if (decodedToken && roles.includes('Admin')) {
                console.log('true');
                isAdmin = true;
            } else {
                isAdmin = false;
            }
        } catch (error) {
            console.error('Error parsing or checking token:', error);
        }
    }

    /**
	 * @param {string} token
	 */
    function parseJwt (token) {
        console.log(token);
        var base64Url = token.split('.')[1];
        var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        var jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));

        return JSON.parse(jsonPayload);
    }
</script>

<div class="container mx-auto">
    <div class="max-w-md mx-auto">
        <h1 class="text-3xl font-semibold mb-4">Login</h1>
        <form>

            <div class="mb-4">
                <label for="username">Username</label>
                <input type="text" id="username" bind:value={username} class="input variant-form-material" required>
            </div>

            <div class="mb-4">
                <label for="password">Password</label>
                <input 
                    type="password" 
                    id="password" 
                    bind:value={password} 
                    class="input variant-form-material"
                    required 
                />
            </div>
                 
            <div class="mb-4">
                <button type="button" on:click={() => login(username, password)} class="btn variant-filled">
                Login
                </button>
            </div>

            {#if loginResult}
                <p class={loginStyle}>{loginResult}</p>
            {/if}
        </form>
    </div>
</div>