// js/gardenSystem.js
import { gameState, updateFluidUI, updateInventoryUI } from './gameState.js';

const GRID_SIZE = 5;

// Define crop types and their properties
const cropData = {
    eye: { fluid: 'sweat', cost: 10, growthTime: 3000, color: 'white', shape: 'circle' },
    hair: { fluid: 'blood', cost: 10, growthTime: 2000, color: '#333', shape: 'patch' },
    finger: { fluid: 'urine', cost: 10, growthTime: 4000, color: '#d1b092', shape: 'rect' }
};

export function setupGardenSystem() {
    const gridContainer = document.getElementById('bio-grid');

    // Generate 5x5 grid
    for (let i = 0; i < GRID_SIZE * GRID_SIZE; i++) {
        const cell = document.createElement('div');
        cell.className = 'grid-cell empty';
        cell.dataset.index = i;

        // Internal state
        cell._cropType = null;
        cell._growthProgress = 0;
        cell._isMature = false;

        // Add canvas for procedural rendering
        const canvas = document.createElement('canvas');
        canvas.width = 50;
        canvas.height = 50;
        cell.appendChild(canvas);

        cell.addEventListener('click', () => handleCellClick(cell));
        gridContainer.appendChild(cell);
    }
}

function handleCellClick(cell) {
    if (cell._isMature) {
        // Harvest
        harvestCrop(cell);
    } else if (cell.classList.contains('empty') && gameState.selectedSeed) {
        // Plant
        plantSeed(cell, gameState.selectedSeed);
    }
}

function plantSeed(cell, seedType) {
    const crop = cropData[seedType];

    // Check fluids
    if (gameState.fluids[crop.fluid] >= crop.cost) {
        // Consume fluid
        gameState.fluids[crop.fluid] -= crop.cost;
        updateFluidUI();

        // Setup cell state
        cell.classList.remove('empty');
        cell.classList.add('growing');
        cell._cropType = seedType;
        cell._growthProgress = 0;
        cell._isMature = false;

        // Start growth loop
        const startTime = Date.now();
        const duration = crop.growthTime;

        const growInterval = setInterval(() => {
            const elapsed = Date.now() - startTime;
            let progress = elapsed / duration;

            if (progress >= 1) {
                progress = 1;
                clearInterval(growInterval);
                cell._isMature = true;
                cell.classList.remove('growing');
                cell.classList.add('mature');
                // Optional: twitching effect when mature
                cell.querySelector('canvas').style.transform = `scale(1) rotate(${Math.random()*10 - 5}deg)`;
            }

            cell._growthProgress = progress;
            renderCrop(cell, seedType, progress);

        }, 100);

        renderCrop(cell, seedType, 0); // Initial render
    } else {
        // Not enough fluid
        cell.style.backgroundColor = '#520';
        setTimeout(() => cell.style.backgroundColor = '', 200);
    }
}

function renderCrop(cell, seedType, progress) {
    const canvas = cell.querySelector('canvas');
    const ctx = canvas.getContext('2d');
    const crop = cropData[seedType];

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Visual scaling based on progress
    // Start small (0.2), grow to full size
    const scale = 0.2 + (0.8 * progress);
    const center = canvas.width / 2;

    ctx.save();
    ctx.translate(center, center);
    ctx.scale(scale, scale);
    ctx.translate(-center, -center);

    if (seedType === 'eye') {
        // Draw Eyeball
        ctx.beginPath();
        ctx.arc(center, center, 20, 0, Math.PI * 2);
        ctx.fillStyle = crop.color;
        ctx.fill();
        ctx.stroke();

        // Iris
        ctx.beginPath();
        ctx.arc(center, center, 8, 0, Math.PI * 2);
        ctx.fillStyle = '#00aaff';
        ctx.fill();

        // Pupil
        ctx.beginPath();
        ctx.arc(center, center, 4, 0, Math.PI * 2);
        ctx.fillStyle = '#000';
        ctx.fill();
    }
    else if (seedType === 'hair') {
        // Draw Hair turf
        ctx.strokeStyle = crop.color;
        ctx.lineWidth = 2;
        for(let i=0; i<30; i++) {
            ctx.beginPath();
            ctx.moveTo(10 + Math.random() * 30, 40);
            ctx.quadraticCurveTo(
                Math.random() * 50, Math.random() * 50,
                10 + Math.random() * 30, 10 + Math.random() * 20
            );
            ctx.stroke();
        }
    }
    else if (seedType === 'finger') {
        // Draw Finger
        ctx.fillStyle = crop.color;
        ctx.fillRect(20, 10, 10, 30);
        ctx.strokeRect(20, 10, 10, 30);

        // Joint lines
        ctx.beginPath();
        ctx.moveTo(20, 20); ctx.lineTo(30, 20);
        ctx.moveTo(20, 30); ctx.lineTo(30, 30);
        ctx.stroke();

        // Nail
        ctx.fillStyle = 'rgba(255,255,255,0.5)';
        ctx.fillRect(22, 12, 6, 6);
    }

    ctx.restore();
}

function harvestCrop(cell) {
    const type = cell._cropType;

    // Add to inventory
    if (gameState.inventory[type] !== undefined) {
        gameState.inventory[type]++;
        updateInventoryUI();
    }

    // Reset cell
    cell._cropType = null;
    cell._growthProgress = 0;
    cell._isMature = false;
    cell.classList.remove('mature');
    cell.classList.add('empty');

    const canvas = cell.querySelector('canvas');
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    canvas.style.transform = 'scale(1) rotate(0deg)';
}
