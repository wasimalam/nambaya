import { EventEmitter, Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { APIUrls } from '@app/shared/Addresses';
export interface MessageReceived {
  patientcaseid: number;
  step: string;
  remainingTime: number;
  totalTimeElapsed: number;
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  data: MessageReceived;
  signalReceived = new EventEmitter<MessageReceived>();

  private hubConnection: signalR.HubConnection;
  private patientServiceUrl = new APIUrls().PatientServiceBaseUrl;
  constructor() {}
  public startConnection = (patientcaseid: number) => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.patientServiceUrl}/quickevaluationprogresshub`)
      // .configureLogging(signalR.LogLevel.Trace)
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.registerForPatient(patientcaseid);
      })
      .catch(err => console.log('Error while starting connection: ', err));
  };
  public addQEPrgoressListener = () => {
    this.hubConnection.on('receiveprogress', data => {
      this.data = data;
      this.signalReceived.emit(data);
    });
  };
  public stopConnection = () => {
    this.hubConnection
      .stop()
      .then(() => console.log('Connection stopped'))
      .catch(err => console.log('Error while stoppping connection: ' + err));
  };
  public registerForPatient = (patientcaseid: number) => {
    this.hubConnection
      .invoke('RegisterForPatient', patientcaseid.toString())
      .then(() => console.log('RegisterForPatient succeeded'))
      .catch(err => console.log('Error while RegisterForPatient : ' + err));
  };
}
