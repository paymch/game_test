// js/gameState.js

export const gameState = {
    fluids: {
        blood: 0,
        sweat: 0,
        urine: 0
    },
    inventory: {
        eye: 0,
        hair: 0,
        finger: 0
    },
    selectedSeed: null,
    selectedInventoryItem: null
};

// UI Update Helpers
export function updateFluidUI() {
    document.getElementById('blood-count').textContent = gameState.fluids.blood;
    document.getElementById('sweat-count').textContent = gameState.fluids.sweat;
    document.getElementById('urine-count').textContent = gameState.fluids.urine;
}

export function updateInventoryUI() {
    const list = document.getElementById('inventory-list');
    list.innerHTML = '';

    for (const [item, count] of Object.entries(gameState.inventory)) {
        if (count > 0) {
            const li = document.createElement('li');
            li.className = 'inv-item';
            li.textContent = `${item.toUpperCase()} (x${count})`;

            if (gameState.selectedInventoryItem === item) {
                li.classList.add('selected');
            }

            li.addEventListener('click', () => {
                gameState.selectedInventoryItem = gameState.selectedInventoryItem === item ? null : item;
                // Deselect seed if inventory selected
                if (gameState.selectedInventoryItem) gameState.selectedSeed = null;

                // Update visuals
                document.querySelectorAll('.seed-btn').forEach(btn => btn.classList.remove('selected'));
                updateInventoryUI();
            });

            list.appendChild(li);
        }
    }
}
