// js/assemblySystem.js
import { gameState, updateInventoryUI } from './gameState.js';
import { evaluateCreature } from './viabilitySystem.js';

export function setupAssemblySystem() {
    const sockets = document.querySelectorAll('.socket');

    sockets.forEach(socket => {
        // Internal state
        socket._slottedItem = null;

        socket.addEventListener('click', () => {
            if (socket._slottedItem) {
                // Remove item
                const item = socket._slottedItem;
                gameState.inventory[item]++;
                updateInventoryUI();

                socket._slottedItem = null;
                socket.classList.remove('filled');
                socket.innerHTML = socket.dataset.type.toUpperCase().replace('_', ' ');

                // Clear any previous status since assembly changed
                document.getElementById('viability-status').textContent = 'DORMANT';
                document.getElementById('countdown-timer').textContent = '';

            } else if (gameState.selectedInventoryItem) {
                // Check if item fits socket (simplified validation for prototype)
                const itemType = gameState.selectedInventoryItem;
                const socketType = socket.dataset.type;

                let isValid = false;
                if (socketType === 'head' && (itemType === 'eye' || itemType === 'hair')) isValid = true;
                // Since we don't have heart/brain seeds yet, allow generic slotting for prototype testing
                if (socketType === 'chest' && itemType === 'eye') isValid = true;
                if (socketType.startsWith('arm') && itemType === 'finger') isValid = true;
                if (socketType.startsWith('leg') && itemType === 'finger') isValid = true;

                if (isValid) {
                    // Consume inventory
                    gameState.inventory[itemType]--;

                    if (gameState.inventory[itemType] <= 0) {
                        gameState.selectedInventoryItem = null;
                    }
                    updateInventoryUI();

                    // Update socket
                    socket._slottedItem = itemType;
                    socket.classList.add('filled');
                    socket.innerHTML = `<strong style="color:#0f0">${itemType.toUpperCase()}</strong>`;

                    // Twitch effect
                    socket.style.transform = `rotate(${Math.random()*20-10}deg)`;
                    setTimeout(() => socket.style.transform = 'rotate(0deg)', 200);
                } else {
                    // Invalid slot
                    socket.style.borderColor = '#f00';
                    setTimeout(() => socket.style.borderColor = '', 200);
                }
            }
        });
    });
}

export function getSocketState(socketId) {
    const socket = document.getElementById(`socket-${socketId}`);
    return socket ? socket._slottedItem : null;
}
