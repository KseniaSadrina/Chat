import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { SCENARIOS } from '../helpers/mocks';
import { Scenario } from '../models/Scenario';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ScenariosService {

  private scenarios = new BehaviorSubject<Scenario[]>([]);
  public scenarios$ = this.scenarios.asObservable();
  scenarioURL = "api/scenarios";

  subscription: Subscription;
  constructor(private httpService: HttpClient) { 
    this.subscription = this.getScenariosFromServer().subscribe(res => {
      this.scenarios.next(res);
      this.subscription.unsubscribe();
    });
  }

  private getScenariosFromServer() : Observable<Scenario[]> {
    return this.httpService.get<Scenario[]>(this.scenarioURL);
  }

  public getScenario(id: number | string): Observable<Scenario> {
    return this.scenarios$.pipe(
      // (+) before `id` turns the string into a number
      map((trainings: Scenario[]) => trainings.find(hero => hero.id === +id))
    );
  }
}
