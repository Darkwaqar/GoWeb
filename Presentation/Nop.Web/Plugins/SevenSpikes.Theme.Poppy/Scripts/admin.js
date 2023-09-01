(function ($) {
    $(document).ready(function () {
        var pavilionSettings = $('#poppy-settings-edit');
        var logoImageId = pavilionSettings.attr('data-logoImageId');
        var presetFieldId = pavilionSettings.attr('data-presetfieldId');

        function logoOverrideChanged(logoImageOverrideId) {
            if ($(logoImageOverrideId).length === 0) {
                return;
            }

            if ($(logoImageOverrideId).is(':checked')) {
                $('#logo-image .upload-image-overlay').remove();
            } else {
                $('#logo-image').append('<div class="upload-image-overlay"></div>');
            }
        }

        function colorPreset(presetOverrideId) {
            if ($(presetOverrideId).length === 0) {
                return;
            }

            if ($(presetOverrideId).is(':checked')) {
                $('.theme-color .adminData .upload-image-overlay').remove();
            } else {
                $('.theme-color .adminData').append('<div class="upload-image-overlay"></div>');
            }
        }

        $(logoImageId).change(logoOverrideChanged);
        $(presetFieldId).change(colorPreset);

        $('.store-scope-configuration .checkbox input').change(function () {
            logoOverrideChanged();
            colorPreset();
        });

        logoOverrideChanged();
        colorPreset();

        var customerPresetObj = new CustomPreset('.theme-color .radionButton:last label', '.theme-color .radionButton label');
        customerPresetObj.setPresetsBackgroundColor();
        customerPresetObj.addKendoColorPickerToTheLastRadioButton();

        $(".theme-color .adminData div").click(function () {
            $(".theme-color .adminData div").removeClass("active");
            $(this).addClass("active");
            // check the current radio button with js, because as we hide the radio buttons they are not checked when clicking on them
            var inputs = $(".theme-color .adminData div input");
            $.each(inputs, function (index, item) {
                item.removeAttribute("checked");
            });
            $(this).find('input').attr("checked", "checked");
        });

        $('.radionButton input[checked=checked]').parent().addClass("active");


    });
})(jQuery);