// js/fluidSystem.js
import { gameState, updateFluidUI } from './gameState.js';

export function setupFluidSystem() {
    const trees = document.querySelectorAll('.fluid-btn');

    trees.forEach(tree => {
        tree.addEventListener('click', (e) => {
            const fluidType = e.target.getAttribute('data-type');
            gameState.fluids[fluidType] += 10;

            // Add a visual juice bounce to the button
            e.target.style.transform = 'scale(1.1)';
            setTimeout(() => e.target.style.transform = 'scale(1)', 100);

            updateFluidUI();
        });
    });
}
