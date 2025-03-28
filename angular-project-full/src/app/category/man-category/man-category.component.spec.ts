import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManCategoryComponent } from './man-category.component';

describe('ManCategoryComponent', () => {
  let component: ManCategoryComponent;
  let fixture: ComponentFixture<ManCategoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManCategoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
