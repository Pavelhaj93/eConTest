export default function Cookie(container) {
    /* Slug to look up in the cookies */
    const agreeSlug = 'privacy_policy';

    /* Check if the customer has previously agreed to Privacy Policy */
    function agreedBefore() {
        const cookies = document.cookie.split('; ');
        const hasAgreed = cookies.filter(entry => entry.includes(agreeSlug))[0];
        return !!hasAgreed;
    }

    /* Check if previously agreed */
    const hasAgreed = agreedBefore();
    !hasAgreed && container.classList.add('privacy-policy-visible');

    /* Handle click */
    const agreeBtn = container.querySelector('.agree');
    agreeBtn.addEventListener('click', (e) => {
        e.preventDefault();

        /* Write to the cookies */
        document.cookie = agreeSlug + '=1; expires=Fri, 31 Dec 9999 23:59:59 GMT; path=/';
        container.classList.remove('privacy-policy-visible');
    });

}