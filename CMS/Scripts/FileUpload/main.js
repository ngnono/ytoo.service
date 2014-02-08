/*
 * jQuery File Upload Plugin JS Example 6.5.1
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/*jslint nomen: true, unparam: true, regexp: true */
/*global $, window, document */

$(function () {
    'use strict';

    // Initialize the jQuery File Upload widget:
    $('#fileupload1').fileupload();
    $('#fileupload-img').fileupload();
    $('#fileupload1').fileupload('option', {
            maxFileSize: 500000000,
            resizeMaxWidth: 1920,
            resizeMaxHeight: 1200,
            maxNumberOfFiles:1,
            acceptFileTypes: /(\.|\/)(xls)$/i,

            done: function (e, data) {
                var that = $(this).data('fileupload'),
                    template,
                    preview,
                    parentNode =$(this);

                if (data.context) {
                    data.context.each(function (index) {
                        var file = ($.isArray(data.result) &&
                                data.result[index]) || { error: 'emptyResult' };
                        if (file.error) {
                            that._adjustMaxNumberOfFiles(1);
                        }
                        that._transition($(this)).done(
                            function () {
                                $(this).remove();
                                template = that._renderDownload(data.result)
                                    .replaceAll(parentNode.find('.files-result'));
                                that._forceReflow(template);
                                that._transition(template).done(
                                    function () {
                                        data.context = $(this);
                                        that._trigger('completed', e, data);
                                    }
                                );
                            }
                        );
                    });
                } else {
                    template = that._renderDownload(data.result)
                        .replaceAll(this.element.find('.files-result'));
                    that._forceReflow(template);
                    that._transition(template).done(
                        function () {
                            data.context = $(this);
                            that._trigger('completed', e, data);
                        }
                    );
                }
            }
    });

    $('#fileupload-img').fileupload('option', {
        sequentialUploads: true,
        limitConcurrentUploads: 1,
        maxFileSize: 500000000,
        resizeMaxWidth: 1920,
        resizeMaxHeight: 1200,
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
        downloadTemplateId: 'template-download-img',
        add: function (e, data) {
            var that = $(this).data('fileupload'),
                options = that.options,
                files = data.files;
            that._adjustMaxNumberOfFiles(-files.length);
            data.isAdjusted = true;

            data.files.valid = data.isValidated = that._validate(files);
            data.context = that._renderUpload(files)
                .appendTo(options.filesContainer)
                .data('data', data);
            that._renderPreviews(files, data.context);
            that._forceReflow(data.context);
            that._transition(data.context).done(
                function () {
                    if ((that._trigger('added', e, data) !== false) &&
                            (options.autoUpload || data.autoUpload) &&
                            data.autoUpload !== false && data.isValidated) {
                        data.submit();
                    }
                }
            );

        },
        fail: function (e, data) {
            var that = $(this).data('fileupload'),
                template;
            if (data.context) {
                data.context.each(function (index) {
                    if (data.errorThrown !== 'abort' &&
                        data.textStatus) {
                        var file = $(this);
                        file.addClass('alert alert-error');
                        that._trigger('failed', e, data);
                    } else {
                        that._transition($(this)).done(
                            function () {
                                $(this).remove();
                                that._trigger('failed', e, data);
                            }
                        );
                    }
                });
            } else if (data.errorThrown !== 'abort') {
                that._adjustMaxNumberOfFiles(data.files.length);
                that._adjustMaxNumberOfFiles(-data.files.length);
                data.context = that._renderUpload(data.files)
                    .appendTo(that.options.filesContainer)
                    .data('data', data);
                that._forceReflow(data.context);
                that._transition(data.context).done(
                    function () {
                        data.context = $(this);
                        that._trigger('failed', e, data);
                    }
                );
            } else {
                that._adjustMaxNumberOfFiles(data.files.length);
                that._trigger('failed', e, data);
            }
        },
        done: function (e, data) {
            var that = $(this).data('fileupload'),
                template,
                preview,
                    parentNode = $(this);
            
            if (data.context) {
                data.context.each(function (index) {
                    var file = ($.isArray(data.result) &&
                            data.result[index]) || { error: 'emptyResult' };
                    if (file.error) {
                        that._adjustMaxNumberOfFiles(1);
                    }
                   
                    that._transition($(this)).done(
                            function () {
                                $(this).remove();
                                template = that._renderDownload(data.result)
                                    .appendTo(parentNode.find('.files-result'));
                                that._forceReflow(template);
                                that._transition(template).done(
                                    function () {
                                        data.context = $(this);
                                        that._trigger('completed', e, data);
                                    }
                                );
                            });
                });
            } else {

                template = that._renderDownload(data.result)
                    .appendTo(this.element.find('.files-result'));
                that._forceReflow(template);
                that._transition(template).done(
                    function () {
                        data.context = $(this);
                        that._trigger('completed', e, data);
                    }
                );
           }
        }
    });
});
