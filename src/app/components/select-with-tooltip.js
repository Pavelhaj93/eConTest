export default function SelectWithTooltip(select) {
    if (!$.fn.tooltip) {
        console.error('Could not initialized tooltip because Bootstrap tooltip function is missing.');
        return;
    }

    let tooltip = select.parentNode.querySelector('.js-tooltip');

    if (!tooltip) {
        return;
    }

    !(tooltip instanceof jQuery) && (tooltip = $(tooltip));

    select.addEventListener('change', (event) => {
        const selectedOption = event.target.options[select.selectedIndex];
        const helpText = selectedOption.dataset.help;

        // change tooltip title according to the selected option
        tooltip.attr('data-original-title', helpText);
    });

    // trigger change event on page load
    const event = document.createEvent('HTMLEvents');
    event.initEvent('change', true, false);
    select.dispatchEvent(event);

    // intialize the tooltip
    tooltip.tooltip();
}