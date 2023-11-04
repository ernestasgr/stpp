<script lang="ts">
	import { onMount } from 'svelte';
	import { accessToken, isAdminStore } from '../../stores';
	import { getToastStore } from '@skeletonlabs/skeleton';

	const toastStore = getToastStore();

	$: movieEdit = {
		id: 0,
		title: '',
		description: '',
		releaseDate: '',
		director: '',
		mainImage: '',
		images: [''] as string[],
		videos: [''] as string[]
	};

	let movies: any[] = [];

	let editResult: string;
	let editSuccessful = false;
	let movieSelect: HTMLSelectElement;
	$: showingTimes = [
		{
			startTime: '2000-01-01T00:00:00Z',
			endTime: '2000-01-01T00:00:00Z',
			price: 0,
			movieId: '',
			number: ''
		}
	];

	let accessTokenValue: string;
	let isAdmin: boolean;

	accessToken.subscribe((value) => {
		accessTokenValue = value;
	});
	isAdminStore.subscribe((value) => {
		isAdmin = value;
	});

	function addImage() {
		movieEdit.images = [...movieEdit.images, ''];
	}

	/**
	 * @param {number} index
	 */
	function removeImage(index: number) {
		movieEdit.images = movieEdit.images.filter((_, i) => i !== index);
	}

	function addVideo() {
		movieEdit.videos = [...movieEdit.videos, ''];
	}

	/**
	 * @param {number} index
	 */
	function removeVideo(index: number) {
		movieEdit.videos = movieEdit.videos.filter((_, i) => i !== index);
	}

	function editMovie() {
		let data = {
			title: movieEdit.title,
			description: movieEdit.description,
			releaseDate: movieEdit.releaseDate + 'T12:00:00Z',
			director: movieEdit.director,
			mainImage: movieEdit.mainImage,
			images: movieEdit.images,
			videos: movieEdit.videos
		};

		fetch(`http://localhost:5157/api/v1/movies/${movieEdit.id}`, {
			method: 'PUT',
			headers: {
				'Content-Type': 'application/json',
				'Authorization': `Bearer ${accessTokenValue}`
			},
			body: JSON.stringify(data)
		})
			.then(async (response) => {
				console.log(response);
				if (!response.ok) {
					editResult = await response.text();
					console.log(editResult);
					editSuccessful = false;
					throw editResult;
				} else {
					return response.json();
				}
			})
			.then((response) => {
				const t = {
					message: 'Movie edited successfully!',
					background: 'variant-filled-success'
				};
				toastStore.trigger(t);
				editSuccessful = true;
				console.log(response);
			})
			.then(async () => {
				for (const showing of showingTimes) {
					console.log(showing);
					let showingData = {
						startTime: showing.startTime + ':00Z',
						endTime: showing.endTime + ':00Z',
						price: showing.price,
						number: showing.number,
						movieId: showing.movieId
					};
					let httpType = showingData.movieId === '' ? 'POST' : 'PUT';
					console.log(showingData);
					await fetch(
						`http://localhost:5157/api/v1/movies/${movieEdit.id}/showings/${showing.number}`,
						{
							method: httpType,
							headers: {
								'Content-Type': 'application/json',
								'Authorization': `Bearer ${accessTokenValue}`
							},
							body: JSON.stringify(showingData)
						}
					).then(async (response) => {
						console.log(response);
						if (!response.ok) {
							throw await response.text();
						} else {
							return response.json();
						}
					});
				}
				const t = {
					message: 'Showings edited successfully!',
					background: 'variant-filled-success'
				};
				toastStore.trigger(t);
			})
			.catch((error) => {
				const t = {
					message: 'Error editing movie! ' + error,
					background: 'variant-filled-error'
				};
				toastStore.trigger(t);
				console.error('Error editing movie', error);
			});
	}

	onMount(() => {
		fetch('http://localhost:5157/api/v1/movies?PageNumber=1&PageSize=50', {
			method: 'GET',
			headers: {
				'Content-Type': 'application/json'
			}
		})
			.then(async (response) => {
				console.log(response);
				if (!response.ok) {
					throw await response.text();
				} else {
					return response.json();
				}
			})
			.then((response) => {
				movies = response;
				console.log(movies);
			})
			.then(() => {
				fetch(`http://localhost:5157/api/v1/movies/${movies[0].id}`, {
					method: 'GET',
					headers: {
						'Content-Type': 'application/json'
					}
				})
					.then(async (response) => {
						console.log(response);
						if (!response.ok) {
							throw await response.text();
						} else {
							return response.json();
						}
					})
					.then((response) => {
						movieEdit = response;
						movieEdit.releaseDate = response.releaseDate.split('T')[0];
						console.log(movieEdit);
					})
					.then(() => {
						fetch(
							`http://localhost:5157/api/v1/movies/${movies[0].id}/showings?PageNumber=1&PageSize=50`,
							{
								method: 'GET',
								headers: {
									'Content-Type': 'application/json'
								}
							}
						)
							.then(async (response) => {
								if (!response.ok) {
									throw await response.text();
								} else {
									return response.json();
								}
							})
							.then((response) => {
								console.log(response);
								response.forEach((element: { endTime: any; startTime: string }) => {
									element.startTime = dateToUserFriendly(element.startTime);
									element.endTime = dateToUserFriendly(element.endTime);
								});
								console.log(response);
								showingTimes = response;
							});
					})
					.catch((error) => console.error('Error fetching movie', error));
			})
			.catch((error) => console.error('Error fetching movies', error));
	});

	function handleSelectChange(event: Event) {
		if (event.target !== null) {
			fetch(`http://localhost:5157/api/v1/movies/${(event.target as HTMLInputElement).value}`, {
				method: 'GET',
				headers: {
					'Content-Type': 'application/json'
				}
			})
				.then(async (response) => {
					console.log(response);
					if (!response.ok) {
						throw await response.text();
					} else {
						return response.json();
					}
				})
				.then((response) => {
					movieEdit = response;
					movieEdit.releaseDate = response.releaseDate.split('T')[0];
					console.log(movieEdit);
				})
				.then(() => {
					fetch(
						`http://localhost:5157/api/v1/movies/${movieEdit.id}/showings?PageNumber=1&PageSize=50`,
						{
							method: 'GET',
							headers: {
								'Content-Type': 'application/json'
							}
						}
					)
						.then(async (response) => {
							if (!response.ok) {
								throw await response.text();
							} else {
								return response.json();
							}
						})
						.then((response) => {
							console.log(response);
							response.forEach((element: { endTime: any; startTime: string }) => {
								element.startTime = dateToUserFriendly(element.startTime);
								element.endTime = dateToUserFriendly(element.endTime);
							});
							console.log(response);
							showingTimes = response;
						});
				})
				.catch((error) => console.error('Error fetching movie', error))
				.catch((error) => console.error('Error fetching movie', error));
		}
	}

	function deleteMovie(id: number) {
		console.log('DELETE');
		fetch(`http://localhost:5157/api/v1/movies/${id}`, {
			method: 'DELETE',
			headers: {
				'Content-Type': 'application/json',
				'Authorization': `Bearer ${accessTokenValue}`
			}
		})
			.then(async (response) => {
				console.log(response);
				if (!response.ok) {
					throw await response.text();
				} else {
					const index = movies.findIndex((movie) => movie.id === movieEdit.id);
					if (index !== -1) {
						movies = movies.filter((_, i) => i !== index);
						const t = {
							message: 'Movie deleted successfully!',
							background: 'variant-filled-success'
						};
						toastStore.trigger(t);
						console.log('Movie deleted from array');
						console.log(movies);
						movieEdit = movies[0];
						movieSelect.value = movies[0].id;
						movieEdit.releaseDate = movies[0].releaseDate.split('T')[0];
					}
				}
			})
			.catch((error) => {
				const t = {
					message: 'Error deleting movie! ' + error,
					background: 'variant-filled-error'
				};
				toastStore.trigger(t);
				console.error('Error deleting movie', error);
			});
	}

	function removeShowingTime(index: number): any {
		console.log(accessTokenValue);
		fetch(
			`http://localhost:5157/api/v1/movies/${movieEdit.id}/showings/${showingTimes[index].number}`,
			{
				method: 'DELETE',
				headers: {
					'Content-Type': 'application/json',
					'Authorization': `Bearer ${accessTokenValue}`
				}
			}
		)
			.then(async (response) => {
				if (!response.ok) {
					throw await response.text();
				} else {
					showingTimes = showingTimes.filter((_, i) => i !== index);
					return response.json();
				}
			})
			.catch((error) => console.error('Error deleting showing', error));
	}

	function addShowingTime() {
		showingTimes = [
			...showingTimes,
			{ startTime: '', endTime: '', movieId: '', number: '', price: 0 }
		];
		console.log(showingTimes);
	}

	function dateToUserFriendly(date: string) {
		let splitDate = date.split('T');
		return splitDate[0] + 'T' + splitDate[1].split(':')[0] + ':' + splitDate[1].split(':')[1];
	}
</script>

{#if isAdmin}
	<select class="select" bind:this={movieSelect} on:change={handleSelectChange}>
		{#each movies as movie}
			<option value={movie.id}>{movie.title}</option>
		{/each}
	</select>

	<main class="p-4">
		<h1 class="text-3xl font-semibold mb-4">Edit Movie Information</h1>

		<form on:submit|preventDefault={editMovie}>
			<div class="mb-4">
				<label for="title" class="block text-sm font-medium mb-2">Title</label>
				<input type="text" id="title" bind:value={movieEdit.title} class="input" />
			</div>

			<div class="mb-4">
				<label for="description" class="block text-sm font-medium mb-2">Description</label>
				<textarea id="description" bind:value={movieEdit.description} class="input" rows="4" />
			</div>

			<div class="mb-4">
				<label for="releaseDate" class="block text-sm font-medium mb-2">Release Date</label>
				<input type="date" id="releaseDate" bind:value={movieEdit.releaseDate} class="input" />
			</div>

			<div class="mb-4">
				<label for="director" class="block text-sm font-medium mb-2">Director</label>
				<input type="text" id="director" bind:value={movieEdit.director} class="input" />
			</div>

			<div class="mb-4">
				<label for="mainImage" class="block text-sm font-medium mb-2">Main Image</label>
				<input type="url" bind:value={movieEdit.mainImage} class="input" />
			</div>

			<div class="mb-4">
				<label for="images" class="block text-sm font-medium mb-2">Images</label>
				{#each movieEdit.images as image, index}
					<div class="flex items-center mb-2">
						<input type="url" bind:value={movieEdit.images[index]} class="input mr-2" />
						<button
							type="button"
							on:click={() => removeImage(index)}
							class="text-red-500 hover:text-red-700">Remove</button
						>
					</div>
					<div class="w-1/2 pl-2 h-auto max-h-[75vh] relative">
						<img
							src={movieEdit.images[index]}
							alt={movieEdit.title}
							style="max-height: 100%; object-fit: contain;"
						/>
					</div>
				{/each}
				<button type="button" on:click={addImage} class="btn btn-sm variant-filled"
					>Add Image</button
				>
			</div>

			<div class="mb-4">
				<label for="videos" class="block text-sm font-medium mb-2">Videos</label>
				{#each movieEdit.videos as video, index}
					<div class="flex items-center mb-2">
						<input type="url" bind:value={movieEdit.videos[index]} class="input mr-2" />
						<button
							type="button"
							on:click={() => removeVideo(index)}
							class="text-red-500 hover:text-red-700">Remove</button
						>
					</div>
					<div class="w-1/2 pl-2 h-auto max-h-[75vh] relative">
						<div class="w-full h-0 pb-[56.25%] bg-black">
							<iframe
								class="absolute inset-0 w-full h-full video-player"
								src={movieEdit.videos[index]}
								frameborder="0"
								allowfullscreen
								title="trailer"
							/>
						</div>
					</div>
				{/each}
				<button type="button" on:click={addVideo} class="btn btn-sm variant-filled"
					>Add Video</button
				>
			</div>

			<div class="flex">
				<div class="mb-4">
					<label for="showingTimes" class="block text-sm font-medium mb-2">Showing Time</label>
					{#each showingTimes as showing, index}
						<div class="flex flex-wrap items-center mb-2">
							<div class="mr-4">
								<label for="startTime" class="block text-xs font-medium mb-1">Start Time</label>
								<input
									type="datetime-local"
									bind:value={showingTimes[index].startTime}
									class="input mb-2"
									placeholder="Start Time"
								/>
							</div>
							<div class="mr-4">
								<label for="endTime" class="block text-xs font-medium mb-1">End Time</label>
								<input
									type="datetime-local"
									bind:value={showingTimes[index].endTime}
									class="input mb-2"
									placeholder="End Time"
								/>
							</div>
							<div class="mr-4">
								<label for="price" class="block text-xs font-medium mb-1">Price</label>
								<input
									type="number"
									bind:value={showingTimes[index].price}
									class="input mb-2"
									placeholder="Price"
								/>
							</div>
							<button
								type="button"
								on:click={() => removeShowingTime(index)}
								class="text-red-500 hover:text-red-700">Remove</button
							>
						</div>
					{/each}
					<button type="button" on:click={addShowingTime} class="btn btn-sm variant-filled"
						>Add Showing</button
					>
				</div>
			</div>

			<div class="flex">
				<button type="submit" class="btn variant-filled mr-2">Edit Movie</button>
				<button
					class="btn variant-filled-primary"
					on:click={(e) => {
						e.preventDefault();
						deleteMovie(movieEdit.id);
					}}>Delete Movie</button
				>
			</div>
		</form>
	</main>
{/if}
