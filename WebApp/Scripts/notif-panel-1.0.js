// A simple templating method for replacing placeholders enclosed in curly braces.
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}

(function ($) {
    var itemTemplate = '<tr data-notification="{Id}">' +
                        '<td>{Id}</td>' +
                        '<td>{Name}</td>' +
                        '<td>' +
                            '<div class="progress progress-striped active">' +
                                '<div class="bar {JobTypeCssClass}" style="width: {FormattedProgress}%;">{FormattedProgress}%</div>' +
                            '</div>' +
                        '</td>' +
                    '</tr>';

    var widgetTemplate = '<a href="#" class="dropdown-toggle" data-toggle="dropdown" aria-expanded="false"><i class="fa fa-tasks"></i><span data-notification="jobsTicker" class="badge blue1">0</span></a>' +
                     '<ul data-notification="jobsList" class="dropdown-menu">' +
                          '<li data-notification="header">' +
                              '<div class="notification_header">' +
                                  '<h3 data-notification="jobsHeader"></h3>' +
                              '</div>' +
                          '</li>' +
                          '<li data-notification="footer">' +
                              '<div class="notification_bottom">' +
                                  '<a data-notification="seeAllJobsLink" href="#">{FooterTmpl}</a>' +
                              '</div>' +
                          '</li>' +
                      '</ul>';


    $.fn.notifPanel = function (jobsEvents, options) {
        var settings = $.extend({
                headerTmpl: "{JobsCount} jobs running",
                footerTmpl: "See all jobs",
                backgroundColor: "white",
                onSeeAllClicked: function() {},
                getProgressBarCssClass: function(job) { return 'blue'; }
            },
            options);

        var tmp = { FooterTmpl: settings.footerTmpl };
        this.html(widgetTemplate.supplant(tmp));

        var jobsCount = 0;
        var jobsList = this.find('ul[data-notification="jobsList"]');
        var jobsTicker = this.find('span[data-notification="jobsTicker"]');
        var jobsHeader = this.find('h3[data-notification="jobsHeader"]');
        var headerLi = jobsList.find('li[data-notification="header"]');
        var footerLi = jobsList.find('li[data-notification="footer"]');

        var seeAllLink = this.find('a[data-notification="seeAllJobsLink"]');
        seeAllLink.click(function () {
            settings.onSeeAllClicked();
        });
        
        function formatJob(job) {
            return $.extend(job, {
                JobTypeCssClass: settings.getProgressBarCssClass(job),
                FormattedProgress: job.Progress.toFixed(1)
            });
        }

        $(jobsEvents).on('updateJob.notifPanel', function (sender, jobStatus) {
            var formattedJobStatus = formatJob(jobStatus);
            var newLi = $(itemTemplate.supplant(formattedJobStatus));

            var foundLi = jobsList.find('li[data-notification="' + jobStatus.Id + '"]');
            if (foundLi.length !== 0) {
                foundLi.replaceWith(newLi);
            } else {
                $(footerLi).before(newLi);

                jobsCount++;
                jobsTicker.text(jobsCount);

                var tmp = { JobsCount: jobsCount };
                jobsHeader.text(settings.headerTmpl.supplant(tmp));
            }
        });

        $(jobsEvents).on('jobComplete.notifPanel', function (sender, jobStatus) {
            var foundLi = jobsList.find('li[data-notification="' + jobStatus.Id + '"]');
            if (foundLi.length !== 0) {
                foundLi.remove();

                jobsCount--;
                jobsTicker.text(jobsCount);
                var tmp = { JobsCount: jobsCount };
                jobsHeader.text(settings.headerTmpl.supplant(tmp));
            }
        });

        $(jobsEvents).on('initJobs.notifPanel', function (sender, jobsArrWrap) {
                jobsCount = jobsArrWrap.jobsArray.length;
                $(jobsList).find('li:not(:first):not(:last)').remove();
                jobsTicker.text(jobsCount);
                var tmp = { JobsCount: jobsCount };
                jobsHeader.text(settings.headerTmpl.supplant(tmp));

                $.each(jobsArrWrap.jobsArray, function () {
                    var job = formatJob(this);
                    $(footerLi).before(itemTemplate.supplant(job));
                });
            });
        return this;
    };

}(jQuery));