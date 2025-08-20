import { CardiologistAccountsService } from '@app/cardiologist/Models/cardiologist-accounts.service';
import { TranslateService, LangChangeEvent } from '@ngx-translate/core';
import { I18nService } from '@app/core';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { forkJoin } from 'rxjs';
import * as moment from 'moment';
import { now } from 'moment';

@Component({
  selector: 'app-cardiologist',
  templateUrl: './cardiologist.component.html',
  styleUrls: ['./cardiologist.component.scss']
})
export class CardiologistComponent implements OnInit {
  public assignedCases: any;
  public activeCases: any;
  public goalsCompletedData: any = {
    goal: '',
    numberCompleted: '',
    percentCompleted: ''
  };
  public goalValueFormatFn = this.goalValueFormat.bind(this);
  public percentageFormattingFn = this.notesPercentageFormat.bind(this);
  public notesValueFormatFn = this.notesValueFormat.bind(this);
  public qeCustomColorsFn = this.qeCustomColors.bind(this);
  public notesCustomColorsFn = this.notesCustomColors.bind(this);
  public yAxisTickFormattingFn = this.yAxisTickFormatting.bind(this);
  public monthlyCasesCompletedData: any;
  public cardiologistNotesData: any;
  public qeStatusData: any;
  public gettingData = true;

  view: any[] = [250, 200];
  showXAxis = true;
  showYAxis = true;
  showXAxisLabel = false;
  xAxisLabel = 'Months';
  showYAxisLabel = false;
  yAxisLabel = 'Cases';
  public gradient = true;
  showLegend = false;
  legendTitle = 'legend';
  legendPosition = 'right';
  guageUnits = '%';
  labelTotal = 'Total';
  colorScheme = {
    domain: ['#604094', '#1074BC']
  };


  qeColorScale = {
    Green: '#92D050', // Green
    Yellow: '#FADD69', // Yellow
    Orange: '#FFAA00', // Orange
    Red: '#D9374D', // Red
    RedRed: '#b40121' // Red Red
  };

  notesColorScale = {
    prescription: '#D9374D',
    noComments: '#604094',
    others: '#1074BC'
  };

  guagecolorScheme = {
    domain: ['#604094', '#1074BC']
  };
  public gaugeData = [];
  public currentLang: string;
  public monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sept', 'Oct', 'Nov', 'Dec'];
  public currentDate = '';

  public caseNotes = [
    { id: 0, description: 'No Comment' },
    { id: 391, description: 'Prescription' },
    { id: 392, description: 'Other' }
  ];
  public qeStatuses = [
    { id: 511, description: 'Green' },
    { id: 512, description: 'Yellow' },
    { id: 513, description: 'Orange' },
    { id: 514, description: 'Red' },
    { id: 515, description: 'Red Red' }
  ];

  constructor(
    private cardiologistService: CardiologistAccountsService,
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
  public goalValueFormat(data, decimals?: number) {
    if (!decimals) {
      decimals = 1;
    }
    const d = Math.pow(10, decimals);

    // tslint:disable-next-line:radix
    return (parseInt(String(this.goalsCompletedData.percentCompleted * d)) / d).toFixed(decimals);
  }

  // tslint:disable-next-line:typedef
  public notesPercentageFormat(data) {
    const decimals = 1;
    const d = Math.pow(10, decimals);

    // tslint:disable-next-line:radix
    return (parseInt(String(data * d)) / d).toFixed(decimals);
  }

  // tslint:disable-next-line:typedef
  public notesValueFormat(data) {
    const decimals = 1;
    const d = Math.pow(10, decimals);

    // tslint:disable-next-line:radix
    return (parseInt(String(data * d)) / d).toFixed(decimals);
  }

  // tslint:disable-next-line:typedef
  onSelect(event) {}

  ngOnInit() {
    const assignedCasesReq = this.cardiologistService.getAssignedCasesStats();
    const activeCasesReq = this.cardiologistService.getActiveCasesStats();
    const goalsCompletedReq = this.cardiologistService.getGoalsCompleted();
    const monthlyCaseCompletedReq = this.cardiologistService.getMonthlyCaseCompleted();
    const cardiologistNoteStatsReq = this.cardiologistService.getCardiologistNoteStats();
    const qeStatusReq = this.cardiologistService.getQEStatusStats();

    forkJoin([
      assignedCasesReq,
      activeCasesReq,
      goalsCompletedReq,
      monthlyCaseCompletedReq,
      cardiologistNoteStatsReq,
      qeStatusReq
    ]).subscribe(results => {
      this.assignedCases = results[0];
      this.activeCases = results[1];
      this.goalsCompletedData = results[2];
      const monthlyCasesCompletedData = results[3];
      const cardiologistNoteStatsData = results[4];
      const qeStatusData = results[5];
      this.cardiologistNotesData = this.makeDataForPieChart(cardiologistNoteStatsData);
      this.qeStatusData = this.makeDataForQEPieChart(qeStatusData);

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
    });

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
  // tslint:disable-next-line:typedef
  qeCustomColors(name) {
    const value = this.qeStatusData.find(s => s.name === name).name;
    if (value.includes('Green') || value.includes('Grün')) {
      return this.qeColorScale.Green;
    } else if (value.includes('Yellow') || value.includes('Gelb')) {
      return this.qeColorScale.Yellow;
    } else if (value.includes('Orange') || value.includes('Orange')) {
      return this.qeColorScale.Orange;
    } else if (value.includes('Red Red:') || value.includes('Rot/Rot:')) {
      return this.qeColorScale.RedRed;
    } else if (value.includes('Red:') || value.includes('Rot:')) {
      return this.qeColorScale.Red;
    }
  }

  yAxisTickFormatting(value) {
    if (value % 1 === 0) {
      return value;
    } else {
      return '';
    }
  }

  // tslint:disable-next-line:typedef
  notesCustomColors(name) {
    const value = this.cardiologistNotesData.find(s => s.name === name).name;
    if (value.includes('Prescription') || value.includes('Verschreibung')) {
      return this.notesColorScale.prescription;
    } else if (value.includes('No Comment') || value.includes('Kein Kommentar')) {
      return this.notesColorScale.noComments;
    } else if (value.includes('Other') || value.includes('Sonstige Befunde')) {
      return this.notesColorScale.others;
    }
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

  private makeDataForPieChart(data: any) {
    const _this = this;
    const actualData = [];
    data.forEach(item => {
      const code = _this.caseNotes.filter(n => n.id === item.code)[0].description;
      _this.translateService.get(code).subscribe(text => {
        const actualObj = {
          name: text,
          value: item.casesCount
        };
        actualData.push(actualObj);
      });
    });

    return actualData;
  }

  private makeDataForQEPieChart(data: any) {
    const _this = this;
    const actualData = [];
    data.forEach(item => {
      const code = _this.qeStatuses.filter(n => n.id === item.qeResultId)[0].description;
      _this.translateService.get(code).subscribe(text => {
        const actualObj = {
          name: text + ': ' + item.casesCount,
          value: item.casesCount
        };
        actualData.push(actualObj);
      });
    });

    return actualData;
  }
}
