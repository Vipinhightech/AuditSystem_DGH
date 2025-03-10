﻿'use strict'; (function (b) {
    b.fn.MultiSelect = function (a) {
        "object" != typeof a && (a = {}); var c = b.extend({ size: 10, css_class_selected: "selected", enableCategoriesSel: !1, keepPrevSelection: !0, allowSubmit: !1 }, a), d = b.fn.MultiSelect.modules, f = {
            _attachSelectionAtOnChange: function () {
                this.change(function () {
                    for (var a = b(this).find(":selected"), c = b.data(this, "MultiSelect_selection"), d = b.data(this, "MultiSelect_clone"), f = 0, e; a[f]; f++) e = b(a[f]), c.toggle(e.get(0)), e.attr("selected", !1), d && (e = d.children()[e.prop("index")],
                    e = b(e), "selected" === e.attr("selected") ? e.removeAttr("selected") : e.attr("selected", "selected"))
                })
            }, _getSelection: function () { return b.data(this.get(0), "MultiSelect_selection") || b.data(this.get(0), "MultiSelect_selection", new d.selection_API(this, c.css_class_selected)) }, _applyCategoriesSelection: function () { new d.categories_selection(this) }, _createClone: function (a) {
                var c = b("<select></select>"); c.prop("id", a.prop("id") + "_clone"); c.attr("name", a.attr("name")); c.attr("multiple", !0); c.html(a.html()); c.hide();
                c.insertAfter(a); a.attr("name", a.attr("name") + "_not_submitted"); a.data("MultiSelect_clone", c)
            }
        }, g = []; this.each(function () { var a = b(this); g.push(f._getSelection.call(a)); f._attachSelectionAtOnChange.call(a); a.attr("multiple") || a.attr("multiple", !0); a.attr("size", c.size); c.enableCategoriesSel && f._applyCategoriesSelection.call(a); c.keepPrevSelection && a.find("option[selected=selected]").addClass(c.css_class_selected); c.allowSubmit && a.attr("name") && f._createClone(a) }); return g
    }; b.fn.MultiSelect.modules =
    {}
})(jQuery); (function () { $.valHooks.select.original_get || ($.valHooks.select.original_get = $.valHooks.select.get, $.valHooks.select.get = function (b) { var a = $.data(b, "MultiSelect_selection"); return a ? (a = a.getAllVal(), b = jQuery.valHooks.select.original_get(b), null == b && (b = []), b = [].concat(a, b), !b.length ? null : b) : jQuery.valHooks.select.original_get(b) }) })(); (function () {
    var b = function (a, b) { this.$select = $(a); this.classSelected = b }; b.prototype.select = function (a) { var b = $(a); if (!b.hasClass(this.classSelected)) return b.addClass(this.classSelected), a }; b.prototype.deselect = function (a) { $(a).removeClass(this.classSelected); return a }; b.prototype.toggle = function (a) { $(a).hasClass(this.classSelected) ? this.deselect(a) : this.select(a) }; b.prototype.getAll = function () { return this.$select.find("option." + this.classSelected) }; b.prototype.resetSelection = function () {
        for (var a =
        0, b = this.getAll() ; this.deselect(b[a]) ; a++);
    }; b.prototype.getAllVal = function () { var a = []; $.each(this.getAll(), function (b, d) { a.push(d.value) }); return a }; $.fn.MultiSelect.modules.selection_API = b
})(); (function () {
    var b = function (a) { this.$trgtSelect = $(a); $('<div class = "multiselect_main_categories"/>').insertBefore(this.$trgtSelect).append(this.$trgtSelect).append(this._getDOMObjWrapper()) }; b.prototype._getDOMObjWrapper = function () {
        var a = $("<div class = 'multiselect_wrap_categories'></div>"), b = [], d = this; $.each(this.$trgtSelect.find("optgroup"), function (a, g) { b[a] = $("<div class = 'category' id = " + g.label + ">" + g.label + "</div>").click(function () { d.selectAllOptionsFromOptgroup(this.id) }) }); $.each(b, function (b,
        c) { a.append(c) }); return a
    }; b.prototype.selectAllOptionsFromOptgroup = function (a) { for (var b = this.$trgtSelect.find("optgroup"), d = 0; b[d] && b[d].label != a; d++); if (d != b.length) { var f = $.data(this.$trgtSelect.get(0), "MultiSelect_selection") || { toggle: function () { } }; $.each($(b[d]).find("option"), function (a, b) { f.toggle(b) }) } }; $.fn.MultiSelect.modules.categories_selection = b
})();
