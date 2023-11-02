<script>
    let email = '';
    let username = '';
    let password = '';
    let confirmPassword = '';

    let registerResult = '';
    let registrationSuccessful = true;

    $: passwordValidationResult = isValidPassword(password, confirmPassword);
    $: passwordStyle = (passwordValidationResult[0]) ? 'input input-success variant-form-material' :
        'input input-error variant-form-material';
    $: passwordErrorMessage = passwordValidationResult[1]  

    $: emailValidationResult = isValidEmail(email);
    $: emailStyle = emailValidationResult ? 'input input-success variant-form-material' :
        'input input-error variant-form-material';

    $: usernameValidationResult = isValidUsername(username);
    $: usernameStyle = usernameValidationResult ? 'input input-success variant-form-material' :
        'input input-error variant-form-material';

    $: registrationStyle = registrationSuccessful ? "variant-ringed-success" : "variant-ringed-error";

    /**
	 * @param {any} username
	 * @param {any} email
	 * @param {any} password
	 */
    function register(username, email, password) {
        if(!passwordValidationResult || !emailValidationResult || !usernameValidationResult) {
            return;
        }
        const data = {
            username: username,
            email: email,
            password: password
        }
        fetch('http://localhost:5157/api/v1/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
        .then(async response => {
            console.log(response);
            if(!response.ok) {
                registerResult = await response.text();
                console.log(registerResult);
                registrationSuccessful = false;
                throw registerResult;
            } else {
                return response.json();
            }
        })
        .then(response => {
            registerResult = 'User registered successfully!';
            registrationSuccessful = true;
            console.log(response);
        })
        .catch((error) => console.error('Error registering', error));
    }

    /**
	 * @param {string} password
	 * @param {string} confirmPassword
	 */
    function isValidPassword(password, confirmPassword) {
        // Define regular expressions to check for each condition
        const lengthRegex = /.{8,}/;
        const uppercaseRegex = /[A-Z]/;
        const lowercaseRegex = /[a-z]/;
        const numberRegex = /[0-9]/;
        const specialSymbolRegex = /[!@#$%^&*()_+{}\[\]:;<>,.?~\\-]/;

        // Check if the password meets all conditions
        const isLengthValid = lengthRegex.test(password);
        if(!isLengthValid) {
            return [false, 'Password must contain at least 8 symbols'];
        }

        const hasLowercase = lowercaseRegex.test(password);
        if(!hasLowercase) {
            return [false, 'Password must contain at least one lowercase letter'];
        }

        const hasUppercase = uppercaseRegex.test(password);
        if(!hasUppercase) {
            return [false, 'Password must contain at least one uppercase letter'];
        }

        const hasNumber = numberRegex.test(password);
        if(!hasNumber) {
            return [false, 'Password must contain at least one number']
        }

        const hasSpecialSymbol = specialSymbolRegex.test(password);
        if(!hasSpecialSymbol) {
            return [false, 'Password must contain at least one special symbol'];
        }

        const passwordsMatch = password === confirmPassword;
        if(!passwordsMatch) {
            return [false, 'Passwords must match'];
        }

        return [true, ''];
    }

    /**
	 * @param {string} email
	 */
    function isValidEmail(email) {
        // Define a regular expression for email validation
        const emailRegex = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/;

        return emailRegex.test(email);
    }

    /**
	 * @param {string} username
	 */
    function isValidUsername(username) {
        return username !== '';
    }
</script>

<div class="container mx-auto">
    <div class="max-w-md mx-auto">
        <h1 class="text-3xl font-semibold mb-4">Register</h1>
        <form>

            <div class="mb-4">
                <label for="username">Username</label>
                <input type="text" id="username" bind:value={username} class={usernameStyle} required>
            </div>

            <div class="mb-4">
                <label for="email">Email</label>
                <input type="email" id="email" bind:value={email} class={emailStyle} required/>
            </div>
            {#if !emailValidationResult}
                <p class="variant-ringed-error">Your email is not valid</p>
                <br>
            {/if}

            <div class="mb-4">
                <label for="password">Password</label>
                <input 
                    type="password" 
                    id="password" 
                    bind:value={password} 
                    class={passwordStyle}
                    required 
                />
            </div>

            <div class="mb-4">
                <label for="confirmPassword">Confirm Password</label>
                <input 
                    type="password" 
                    id="confirmPassword" 
                    bind:value={confirmPassword} 
                    class={passwordStyle}
                    required 
                />
            </div>
            {#if passwordErrorMessage}
                <p class="variant-ringed-error">{passwordErrorMessage}</p>
                <br>
            {/if}
            
            
            <div class="mb-4">
                <button type="button" on:click={() => register(username, email, password)} class="btn variant-filled">
                Register
                </button>
            </div>

            {#if registerResult}
                <p class={registrationStyle}>{registerResult}</p>
            {/if}
        </form>
    </div>
</div>