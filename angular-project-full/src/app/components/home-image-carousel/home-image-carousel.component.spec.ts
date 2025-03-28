import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeImageCarouselComponent } from './home-image-carousel.component';

describe('HomeImageCarouselComponent', () => {
  let component: HomeImageCarouselComponent;
  let fixture: ComponentFixture<HomeImageCarouselComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomeImageCarouselComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HomeImageCarouselComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
