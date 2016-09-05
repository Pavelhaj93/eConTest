export default function Copyright(container) {
    const beginYear = 2016;
    let output = [beginYear];
    const date = new Date();
    const currentYear = date.getFullYear();

    /* Append currentYear if it exeeds the beginYear */
    (currentYear > beginYear) && output.push(currentYear);

    /* Output */
    container.innerHTML = output.join(' - ');
}