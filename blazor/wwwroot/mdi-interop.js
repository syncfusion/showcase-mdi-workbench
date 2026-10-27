// Minimal JS interop helpers for the MDI workspace shell.
window.mdiWorkspace = {

    getSize: function (elementId) {
        const el = document.getElementById(elementId);
        if (!el) {
            return { width: 1024, height: 640 };
        }
        return { width: el.clientWidth, height: el.clientHeight };
    },


    getBounds: function (elementId) {
        const el = document.getElementById(elementId);
        if (!el) {
            return { left: 0, top: 0, width: 0, height: 0 };
        }
        const left = parseFloat(el.style.left);
        const top = parseFloat(el.style.top);
        return {
            left: Number.isFinite(left) ? left : el.offsetLeft,
            top: Number.isFinite(top) ? top : el.offsetTop,
            width: el.offsetWidth,
            height: el.offsetHeight
        };
    },

    setZIndex: function (elementId, zIndex) {
        const el = document.getElementById(elementId);
        if (!el || typeof zIndex !== 'number' || !isFinite(zIndex)) {
            return false;
        }
        el.style.zIndex = zIndex;
        return true;
    },

    setBounds: function (elementId, left, top) {
        const el = document.getElementById(elementId);
        if (!el) {
            return false;
        }
        if (typeof left === 'number' && isFinite(left)) {

            el.style.left = left + 'px';
        }
        if (typeof top === 'number' && isFinite(top)) {
            el.style.top = top + 'px';
        }
        return true;
    },


    observeViewportSize: function (elementId, dotnetRef) {
        const el = document.getElementById(elementId);
        const noop = { dispose: function () { /* no-op */ } };
        if (!el || typeof ResizeObserver === 'undefined') {
            return noop;
        }
        const observer = new ResizeObserver(function (entries) {
            for (var i = 0; i < entries.length; i++) {
                var size = entries[i].contentRect;
                try {
                    dotnetRef.invokeMethodAsync(
                        'OnViewportResizedAsync',
                        Math.round(size.width),
                        Math.round(size.height)
                    );
                } catch (e) {
                    // Disposed/ref-disposed edge cases — swallow.
                }
            }
        });
        observer.observe(el);
        return { dispose: function () { observer.disconnect(); } };
    },
    notifyDialogResized: function (elementId) {
        const el = document.getElementById(elementId);
        if (!el) {
            return false;
        }

        try {
            const content = el.querySelector('.e-dlg-content') || el;
            content.dispatchEvent(new Event('resize', { bubbles: true }));
        } catch (e) {
            // Ignore.
        }
        try {
            window.dispatchEvent(new Event('resize'));
        } catch (e) {
            // Ignore.
        }
        return true;
    }
};
