// js/main.js
import { gameState, updateFluidUI, updateInventoryUI } from './gameState.js';
import { setupFluidSystem } from './fluidSystem.js';
import { setupGardenSystem } from './gardenSystem.js';
import { setupAssemblySystem } from './assemblySystem.js';
import { evaluateCreature } from './viabilitySystem.js';

document.addEventListener('DOMContentLoaded', () => {
    // Initialize Systems
    setupFluidSystem();
    setupGardenSystem();
    setupAssemblySystem();

    // Initial UI Update
    updateFluidUI();
    updateInventoryUI();

    // Bind Seed Selection Buttons
    const seedBtns = document.querySelectorAll('.seed-btn');
    seedBtns.forEach(btn => {
        btn.addEventListener('click', (e) => {
            const seedType = e.target.getAttribute('data-seed');

            // Toggle selection
            if (gameState.selectedSeed === seedType) {
                gameState.selectedSeed = null;
                e.target.classList.remove('selected');
            } else {
                gameState.selectedSeed = seedType;
                // Deselect inventory item if seed selected
                if (gameState.selectedSeed) {
                    gameState.selectedInventoryItem = null;
                    updateInventoryUI(); // refreshes rendering to drop selection class
                }

                // Update button visuals
                seedBtns.forEach(b => b.classList.remove('selected'));
                e.target.classList.add('selected');
            }
        });
    });

    // Bind Animate Button
    const animateBtn = document.getElementById('animate-btn');
    animateBtn.addEventListener('click', () => {
        evaluateCreature();
    });
});
