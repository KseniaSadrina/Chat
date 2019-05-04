import { Component, OnInit, OnDestroy } from '@angular/core';
import { TrainingsService } from 'src/app/services/trainings.service';
import { Observable, Subscription, BehaviorSubject } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { Training } from 'src/app/models/Training';
import { ActivatedRoute } from '@angular/router';
import { AddTrainingComponent } from '../add-training/add-training.component';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { prototype } from 'events';
import { ME } from 'src/app/helpers/mocks';
import { User } from 'src/app/models/User';

export const DIALOG = 'dialog';
export const SERVICE = 'service';

@Component({
  selector: 'app-trainings-list',
  templateUrl: './trainings-list.component.html',
  styleUrls: ['./trainings-list.component.css']
})
export class TrainingsListComponent implements OnInit{

  constructor(private trainingsService: TrainingsService, private route: ActivatedRoute, public dialog: MatDialog) { }

  public trainings: Observable<Training[]>;
  public selectedId: Observable<number>;
  public title = 'Trainings';
  public user = new BehaviorSubject<User>(ME);

  ngOnInit() {
    this.trainings = this.trainingsService.trainings$;
    this.selectedId = this.trainingsService.currentIndex$;
  }

  openDialog(): void {
    this.dialog.open(AddTrainingComponent);
  }

}
