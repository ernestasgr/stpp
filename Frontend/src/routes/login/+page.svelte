<script>
	import { accessToken, usernameStore } from "../../stores";

    let username = '';
    let password = '';
    let loginResult = '';
    let loginSuccessful = false;
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
            loginSuccessful = true;
            console.log(response);
        })
        .catch((error) => console.error('Error logging in', error));
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