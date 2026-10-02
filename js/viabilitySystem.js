// js/viabilitySystem.js
import { gameState } from './gameState.js';
import { getSocketState } from './assemblySystem.js';

let terminalInterval = null;
let remainingTime = 60;

export function evaluateCreature() {
    const statusText = document.getElementById('viability-status');
    const timerText = document.getElementById('countdown-timer');

    // Clear previous timer
    if (terminalInterval) {
        clearInterval(terminalInterval);
        terminalInterval = null;
    }
    timerText.textContent = '';

    const head = getSocketState('head');
    const chest = getSocketState('chest');
    const armL = getSocketState('arm-l');
    const armR = getSocketState('arm-r');
    const legL = getSocketState('leg-l');
    const legR = getSocketState('leg-r');

    // Define essential core life checks
    // We assume an Eye or Hair in head might technically "live" briefly, but ideally we need Brain + Heart.
    // For this port, let's treat any slotted item in Head and Chest as meeting bare minimum life threshold to react.
    const hasCore = (head !== null && chest !== null);
    const hasAllLimbs = (armL !== null && armR !== null && legL !== null && legR !== null);
    const hasAnyLimb = (armL !== null || armR !== null || legL !== null || legR !== null);

    if (!head && !chest) {
        statusText.textContent = "INANIMATE MEAT";
        statusText.style.color = "#888";
        return;
    }

    if (hasCore) {
        if (hasAllLimbs) {
            statusText.textContent = "ALIVE & STABLE";
            statusText.style.color = "#0f0";
        } else {
            statusText.textContent = "CRIPPLED (PERMANENT)";
            statusText.style.color = "#aa0";
        }
    } else {
        // Missing core organs but has something attached
        statusText.textContent = "TERMINAL SHOCK";
        statusText.style.color = "#f00";

        // Start countdown
        remainingTime = 60;
        timerText.textContent = `Expires in: ${remainingTime}s`;

        terminalInterval = setInterval(() => {
            remainingTime--;
            if (remainingTime <= 0) {
                clearInterval(terminalInterval);
                statusText.textContent = "EXPIRED";
                statusText.style.color = "#555";
                timerText.textContent = '';
                // Optional: trigger explosion or reset
            } else {
                timerText.textContent = `Expires in: ${remainingTime}s`;
            }
        }, 1000);
    }
}
