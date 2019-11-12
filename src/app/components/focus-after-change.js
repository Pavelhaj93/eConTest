export default function SetFocusAfterChange(element) {
    const targetId = element.dataset.target;
    const target = document.getElementById(targetId);

    if (!target) {
        return;
    }

    element.addEventListener('change', () => {
        target.focus();
    });
}