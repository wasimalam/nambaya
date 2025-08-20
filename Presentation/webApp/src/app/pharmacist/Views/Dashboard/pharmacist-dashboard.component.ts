import { PharmacistService } from '@app/pharmacist/Models/pharmacist.service';
import { TranslateService, LangChangeEvent } from '@ngx-translate/core';
import { I18nService } from '@app/core';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { forkJoin } from 'rxjs';
import * as moment from 'moment';
import { now } from 'moment';

@Component({
  selector: 'app-pharmacist-dashboard',
  templateUrl: './pharmacist-dashboard.component.html',
  styleUrls: ['./pharmacist-dashboard.component.scss']
})
export class PharmacistDashboardComponent implements OnInit {
  public pharmacyStatsData: any;
  public goalsCompletedData: any = {
    goal: '',
    numberCompleted: '',
    percentCompleted: ''
  };
  public goalValueFormatFn = this.goalValueFormat.bind(this);
  public yAxisTickFormattingFn = this.yAxisTickFormatting.bind(this);
  public monthlyCasesStartedData: any;
  public monthlyCasesCompletedData: any;
  public gettingData = true;
  public isLoading = true;

  view: any[] = [570, 250];
  showXAxis = true;
  showYAxis = true;
  showXAxisLabel = false;
  xAxisLabel = 'Months';
  showYAxisLabel = false;
  yAxisLabel = 'Cases';
  public gradient = true;
  legendTitle = 'legend';
  showLegend = false;
  guageUnits = '%';
  colorScheme = {
    domain: ['#604094', '#1074BC']
  };


  guagecolorScheme = {
    domain: ['#604094', '#1074BC']
  };
  public gaugeData = [];
  public currentLang: string;
  public monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sept', 'Oct', 'Nov', 'Dec'];
  public currentDate = '';

  constructor(
    private pharmacistService: PharmacistService,
    private i18nService: I18nService,
    public translateService: TranslateService,
    private _changeDetectorRef: ChangeDetectorRef
  ) {
    translateService.onLangChange.subscribe((event: LangChangeEvent) => {
      this.currentLang = event.lang;
      // TODO This as a workaround.
      this._changeDetectorRef.detectChanges();
    });
  }

  // tslint:disable-next-line:typedef
  onSelect(event) {}

  yAxisTickFormatting(value) {
    if (value % 1 === 0) {
      return value;
    } else {
      return '';
    }
  }

  // tslint:disable-next-line:typedef
  public goalValueFormat(data, decimals?: number) {
    if (!decimals) {
      decimals = 1;
    }
    const d = Math.pow(10, decimals);

    // tslint:disable-next-line:radix
    return (parseInt(String(this.goalsCompletedData.percentCompleted * d)) / d).toFixed(decimals);
  }

  ngOnInit() {
    const pharmacyStatsReq = this.pharmacistService.getPharmacyStats();
    const goalsCompletedReq = this.pharmacistService.getGoalsCompleted();
    const monthlyCaseStartedReq = this.pharmacistService.getMonthlyCaseStarted();
    const monthlyCaseCompletedReq = this.pharmacistService.getMonthlyCaseCompleted();

    forkJoin([pharmacyStatsReq, goalsCompletedReq, monthlyCaseStartedReq, monthlyCaseCompletedReq]).subscribe(
      results => {
        this.pharmacyStatsData = results[0];
        this.goalsCompletedData = results[1];
        const monthlyCasesStartedData = results[2];
        const monthlyCasesCompletedData = results[3];
        this.monthlyCasesStartedData = this.makeDataForBarChart(monthlyCasesStartedData);
        this.monthlyCasesCompletedData = this.makeDataForBarChart(monthlyCasesCompletedData);

        const gaugeChartData = [];
        this.translateService.get('Target').subscribe(text => {
          const guageObj = {
            name: text + ': ' + this.goalsCompletedData.goal.toLocaleString(this.translateService.currentLang),
            value: this.goalsCompletedData.goal
          };
          gaugeChartData.push(guageObj);
        });
        this.translateService.get('Status').subscribe(text => {
          const guageObj = {
            name: text + ': ' + this.goalsCompletedData.numberCompleted,
            value: this.goalsCompletedData.numberCompleted
          };
          gaugeChartData.push(guageObj);
        });

        this.gaugeData = gaugeChartData;
        this.gettingData = false;
        this.isLoading = false;
      }
    );

    const dateFormatter = setInterval(() => {
      const dateFormat = localStorage.getItem('dateFormat');
      if (dateFormat) {
        this.formatDate();
        clearInterval(dateFormatter);
      }
    }, 500);
  }

  formatDate() {
    const dateFormat = localStorage.getItem('dateFormat');
    const timeFormat = localStorage.getItem('timeFormat');
    this.currentDate = `${moment(now()).format(dateFormat + ' ' + timeFormat)}`;
  }

  private makeDataForBarChart(data: any) {
    const _this = this;
    const actualData = [];
    data.forEach(item => {
      const month = _this.monthNames[item.month - 1];
      _this.translateService.get(month).subscribe(text => {
        const actualObj = {
          name: `${text},${item.year}`,
          value: item.casesCount
        };
        actualData.push(actualObj);
      });
    });
    return actualData;
  }
}
