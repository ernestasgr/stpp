<script>
	import { getModalStore } from '@skeletonlabs/skeleton';
	import { accessToken } from './stores';
	import { onMount } from 'svelte';

	// Define the data for your seating grid, assuming it's a 2D array.
	let rows = 5;
	let columns = 10;
	/**
	 * @type {any[]}
	 */
	let seatingGrid = [];
	/**
	 * @type {any}
	 */
	let accessTokenValue;
	/**
	 * @type {any[]}
	 */
	let boughtTickets = [];

	accessToken.subscribe((value) => {
		accessTokenValue = value;
	});

	const modalStore = getModalStore();

	for (let i = 0; i < rows; i++) {
		let row = [];
		for (let j = 0; j < columns; j++) {
			row.push({
				row: i + 1,
				column: j + 1,
				isOccupied: false
			});
		}
		seatingGrid.push(row);
	}

	// Function to toggle the seat occupancy
	/**
	 * @param {number} row
	 * @param {number} column
	 */
	function toggleSeat(row, column) {
		let seat = row + 1 + '-' + (column + 1);
		console.log(seat);
		console.log(boughtTickets);
		let containsSeat = boughtTickets.some((t) => t.seat === seat);
		console.log(containsSeat);
		if (!containsSeat) {
			seatingGrid[row][column].isOccupied = !seatingGrid[row][column].isOccupied;
		}
	}

	async function buy() {
		let movieId = $modalStore[0].meta.movieId;
		let showingId = $modalStore[0].meta.showingId;
		for (let i = 0; i < seatingGrid.length; i++) {
			for (let j = 0; j < seatingGrid[i].length; j++) {
				if (seatingGrid[i][j].isOccupied) {
					let data = {
						price: 5,
						ticketType: 0,
						seat: i + 1 + '-' + (j + 1)
					};
					await fetch(
						`http://localhost:5157/api/v1/movies/${movieId}/showings/${showingId}/tickets`,
						{
							method: 'POST',
							headers: {
								'Content-Type': 'application/json',
								Authorization: `Bearer ${accessTokenValue}`
							},
							body: JSON.stringify(data)
						}
					)
						.then(async (response) => {
							if (!response.ok) {
								throw await response.text();
							} else {
								return response.json();
							}
						})
						.catch((error) => console.error('Error buying ticket', error));
				}
			}
		}

		modalStore.close();
	}

	onMount(() => {
		let movieId = $modalStore[0].meta.movieId;
		let showingId = $modalStore[0].meta.showingId;
		fetch(
			`http://localhost:5157/api/v1/movies/${movieId}/showings/${showingId}/tickets?PageNumber=1&PageSize=50`,
			{
				method: 'GET',
				headers: {
					'Content-Type': 'application/json',
					Authorization: `Bearer ${accessTokenValue}`
				}
			}
		)
			.then(async (response) => {
				if (!response.ok) {
					throw await response.text();
				} else {
					boughtTickets = await response.json();
					boughtTickets.forEach((ticket) => {
						let splitTicket = ticket.seat.split('-');
						seatingGrid[splitTicket[0] - 1][splitTicket[1] - 1].isBought = true;
					});
					console.log(boughtTickets);
					return boughtTickets;
				}
			})
			.catch((error) => console.error('Error getting tickets', error));
	});
</script>

<!-- HTML structure for the modal -->
<div
	class="fixed top-0 left-0 w-full h-full flex justify-center items-center bg-black bg-opacity-60 z-50"
>
	<div class="w-11/12 max-w-2xl p-4 rounded-md shadow-lg">
		<div class="text-2xl font-semibold mb-4">Seating Grid</div>

		<!-- Loop through the seating grid and create seats -->
		<div class="grid grid-cols-5 md:grid-cols-10 gap-2">
			{#each seatingGrid as row, i}
				{#each row as seat, j}
					<div
						class="w-10 h-10 flex justify-center items-center border border-gray-500 rounded-md cursor-pointer"
						class:bg-green-600={seat.isOccupied}
						class:variant-filled-primary={seat.isBought}
						on:click={() => toggleSeat(i, j)}
					>
						{seat.row}-{seat.column}
					</div>
				{/each}
			{/each}
		</div>

		<!-- Close button -->
		<button class="mt-4 variant-filled-primary py-2 px-4 rounded-md" on:click={() => buy()}>
			Buy
			<i class="fa-solid fa-money-bill" />
		</button>
		{#if $modalStore[0]}
			<p>{JSON.stringify($modalStore[0].meta)}</p>
		{/if}
	</div>
</div>
