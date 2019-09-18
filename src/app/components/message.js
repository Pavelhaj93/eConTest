/*
*
* Message
*
* Messages are used as an output in various AJAX result containers
* to display the warning/error during the AJAX call.
*
* ARGUMENTS:
* container (DOMElement/jQuery) - Container to append/prepend the message to.
* name (String) - Message name/type to look in the "messages" Object.
* prepend (Boolean) - Flag if message should be prepended to container. Default is append.
*
*/

export default function Message(container, name, prepend = false) {
    /* Stop when no messages are passed from Back-end */
    if (!messages) { return; }

    !(container instanceof jQuery) && (container = $(container));

    /* Get the required message */
    const message = messages[name].join('');

    /* Append or prepend to container */
    if (prepend) {
        container.prepend(message);
    } else {
        container.append(message);
    }
}
