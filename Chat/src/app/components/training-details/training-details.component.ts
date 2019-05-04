import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { TrainingsService } from 'src/app/services/trainings.service';
import { Observable, Subscription, BehaviorSubject,  } from 'rxjs';
import { Training } from 'src/app/models/Training';
import { combineLatest, map, switchMap, merge } from 'rxjs/operators';
import { SessionsService } from 'src/app/services/sessions.service';
import { ChatSession } from 'src/app/models/chatSession';

@Component({
  selector: 'app-training-details',
  templateUrl: './training-details.component.html',
  styleUrls: ['./training-details.component.css']
})
export class TrainingDetailsComponent implements OnInit {

  training: Observable<Training>;
  session: Observable<ChatSession>;

  constructor(private sessions: SessionsService, private trainings: TrainingsService) { }

  ngOnInit() {
    this.training = this.trainings.currentTraining$;
    this.session = this.sessions.currentSession$;
  }

  public joinSession(trainingId: number) {
    this.sessions.joinToSession(trainingId);
  }

  public leaveSession(sessionName: string) {
    this.sessions.leaveSession(sessionName);
  }

}
