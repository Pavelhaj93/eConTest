export default function Tooltip(element) {
    !(element instanceof jQuery) && (element = $(element));
    if ($.fn.tooltip) {
        element.tooltip();
    } else {
        console.error('Could not initialized tooltip because Bootstrap tooltip function is missing.');
    }
}