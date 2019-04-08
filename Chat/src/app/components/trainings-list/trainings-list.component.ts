import { Component, OnInit, OnDestroy } from '@angular/core';
import { TrainingsService } from 'src/app/services/trainings.service';
import { Observable, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { Training } from 'src/app/models/Training';
import { ActivatedRoute } from '@angular/router';
import { AddTrainingComponent } from '../add-training/add-training.component';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { prototype } from 'events';

export const DIALOG = "dialog";
export const SERVICE = "service";

@Component({
  selector: 'app-trainings-list',
  templateUrl: './trainings-list.component.html',
  styleUrls: ['./trainings-list.component.css']
})
export class TrainingsListComponent implements OnInit, OnDestroy {

  constructor(private trainingsService: TrainingsService, private route: ActivatedRoute, public dialog: MatDialog) { }

  trainings: Observable<Training[]>;
  selectedId: number;
  public title = "Trainings";
  
  private subscriptions:Subscription[] = [];

  ngOnInit() {
    this.trainings = this.route.paramMap.pipe(
      switchMap(params => {
        this.selectedId = +params.get('id');
        return this.trainingsService.trainings$;
      })
    );
    this.subscriptions.push(this.trainings.subscribe());
  }

  ngOnDestroy(): void {
    try {
      this.subscriptions.forEach(item => item.unsubscribe());
    } catch {
      console.log("Caught unsubscribe error.");
    }
  }

  openDialog(): void {
    this.dialog.open(AddTrainingComponent);
  }

}
