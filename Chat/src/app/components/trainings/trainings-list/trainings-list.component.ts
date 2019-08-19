import { Component, OnInit, OnDestroy } from '@angular/core';
import { TrainingsService } from 'src/app/services/trainings.service';
import { Observable, Subscription, BehaviorSubject } from 'rxjs';
import { Training } from 'src/app/models/Training';
import { MatDialog } from '@angular/material';
import { User } from 'src/app/models/User';
import { AddTrainingComponent } from '../add-training/add-training.component';
import { CustomAuthService } from 'src/app/services/custom-auth.service';
import { SessionsService } from 'src/app/services/sessions.service';

export const DIALOG = 'dialog';
export const SERVICE = 'service';

@Component({
  selector: 'app-trainings-list',
  templateUrl: './trainings-list.component.html',
  styleUrls: ['./trainings-list.component.css']
})
export class TrainingsListComponent implements OnInit {

  constructor(private trainingsService: TrainingsService,
              private authService: CustomAuthService,
              public dialog: MatDialog,
              public sessions: SessionsService) { }

  public trainings: Observable<Training[]>;
  public unreadMessages: Observable<{ [trainingId: number]: number }>;
  public selectedId: Observable<number>;
  public title = 'Trainings';
  public user: Observable<User>;

  ngOnInit() {
    this.trainings = this.trainingsService.trainings$;
    this.selectedId = this.trainingsService.currentIndex$;
    this.user = this.authService.currentUser$;
    this.unreadMessages = this.sessions.unreadMessages$;
    this.unreadMessages.subscribe(res => {
      console.log(res);
    });
  }

  openDialog(): void {
    this.dialog.open(AddTrainingComponent);
  }

}
