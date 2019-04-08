import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { TrainingsService } from 'src/app/services/trainings.service';
import { Observable, Subscription, BehaviorSubject } from 'rxjs';
import { Training } from 'src/app/models/Training';
import { switchMap, map } from 'rxjs/operators';
import { SessionsService } from 'src/app/services/sessions.service';
import { ChatSession } from 'src/app/models/chatSession';

@Component({
  selector: 'app-training-details',
  templateUrl: './training-details.component.html',
  styleUrls: ['./training-details.component.css']
})
export class TrainingDetailsComponent implements OnInit, OnDestroy {
  
  training: Observable<Training>;
  isSessionActive = new BehaviorSubject<boolean>(false);
  subscriptions: Subscription[] = [];


  constructor(
    private route: ActivatedRoute,
    private service: TrainingsService,
    private sessions: SessionsService
    ) { }
    
    ngOnInit() {
      this.training = this.route.paramMap.pipe(
        switchMap((params: ParamMap) =>
        this.service.getTraining(params.get('id')))
        );

        let sub = this.sessions.currentSession$.pipe(
          map((activeSession: ChatSession) => {
            this.isSessionActive.next(activeSession !== null && activeSession !== undefined);
          })
        ).subscribe();
        
        this.subscriptions.push(this.training.subscribe());
        this.subscriptions.push(sub);
      }
    
    ngOnDestroy(): void {
      this.subscriptions.forEach(item => item.unsubscribe());
    }

    public joinSession() {
      
      let sub = this.training.pipe(
        map(training => this.sessions.joinSession(training.id))
      ).subscribe( res => {
        if (sub) sub.unsubscribe();
      });
    }

    public leaveSession() {
      let sub = this.sessions.currentSession$.pipe(
        map((session: ChatSession) => {
          if (!session) return;
          this.sessions.leaveSession(session.name);
        })
      ).subscribe( res => {
        if (sub) sub.unsubscribe();
      });
  }

}
