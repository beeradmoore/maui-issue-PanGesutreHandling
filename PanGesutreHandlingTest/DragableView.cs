using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanGesutreHandlingTest;

internal class DragableView : Grid, INotifyPropertyChanged 
{
    public event PropertyChangedEventHandler PropertyChanged;


    bool isMainGestureRunning = false;
    public bool IsMainGestureRunning
    {
        get => isMainGestureRunning;
        set
        {
            if (isMainGestureRunning != value)
            {
                isMainGestureRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMainGestureRunning)));
            }
        }
    }


    bool isHorizontalGestureRunning = false;
    public bool IsHorizontalGestureRunning
    {
        get => isHorizontalGestureRunning;
        set
        {
            if (isHorizontalGestureRunning != value)
            {
                isHorizontalGestureRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHorizontalGestureRunning)));
            }
        }
    }


    bool isVerticalGestureRunning = false;
    public bool IsVerticalGestureRunning
    {
        get => isVerticalGestureRunning;
        set
        {
            if (isVerticalGestureRunning != value)
            {
                isVerticalGestureRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVerticalGestureRunning)));
            }
        }
    }



    double lastMainTotalX = 0.0;
    double lastMainTotalY = 0.0;
    double lastHorizontalTotalX = 0.0;
    double lastVerticalTotalY = 0.0;

    public DragableView()
    {
        // Make it aligned to start for hor/ver so we can move it with TranslationX/Y
        HorizontalOptions = LayoutOptions.Start;
        VerticalOptions = LayoutOptions.Start;

        WidthRequest = 200;
        HeightRequest = 200;
        TranslationX = 20;
        TranslationY = 20;
        BackgroundColor = Color.FromRgb(69, 127, 165);

        ColumnDefinitions = new ColumnDefinitionCollection()
        {
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
        };

        var leftBox = new Grid()
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 75,
            HeightRequest = 75,
            BackgroundColor = Color.FromRgb(218, 240, 255)
        };
        var leftLabel = new Label()
        {
            Text = "H",
            TextColor = Colors.Black,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };
        leftBox.Children.Add(leftLabel);
        Grid.SetColumn(leftBox, 0);
        Children.Add(leftBox);


        var rightBox = new Grid()
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 75,
            HeightRequest = 75,
            BackgroundColor = Color.FromRgb(181, 225, 255)
        };
        var rightLabel = new Label()
        {
            Text = "V",
            TextColor = Colors.Black,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };
        rightBox.Children.Add(rightLabel);
        Grid.SetColumn(rightBox, 1);
        Children.Add(rightBox);


        var mainPanGestureRecognizer = new PanGestureRecognizer();
        mainPanGestureRecognizer.PanUpdated += MainPanGestureRecognizer_PanUpdated;
        GestureRecognizers.Add(mainPanGestureRecognizer);


        var horizontalPanGestureRecognizer = new PanGestureRecognizer();
        horizontalPanGestureRecognizer.PanUpdated += HorizontalPanGestureRecognizer_PanUpdated;
        leftBox.GestureRecognizers.Add(horizontalPanGestureRecognizer);


        var verticalPanGestureRecognizer = new PanGestureRecognizer();
        verticalPanGestureRecognizer.PanUpdated += VerticalPanGestureRecognizer_PanUpdated;
        rightBox.GestureRecognizers.Add(verticalPanGestureRecognizer);
    }


    private void MainPanGestureRecognizer_PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        Debug.WriteLine($"MainPanGesture - {e.StatusType}");

        if (e.StatusType == GestureStatus.Started)
        {
            IsMainGestureRunning = false;

            lastMainTotalX = 0.0;
            lastMainTotalY = 0.0;
        }
        else if (e.StatusType == GestureStatus.Running)
        {
            IsMainGestureRunning = true;

            var deltaX = e.TotalX - lastMainTotalX;
            var deltaY = e.TotalY - lastMainTotalY;

            var tempX = TranslationX + deltaX;
            var tempY = TranslationY + deltaY;

            // Keep it within min bounds
            if (tempX < 0)
            {
                tempX = 0;
            }

            if (tempY < 0)
            {
                tempY = 0;
            }

            // Keep it within max bounds
            if (Parent is Grid parentGrid)
            {
                if (tempX + Width > parentGrid.Width)
                {
                    tempX = parentGrid.Width - Width;
                }

                if (tempY + Height > parentGrid.Height)
                {
                    tempY = parentGrid.Height - Height;
                }
            }

            TranslationX = tempX;
            TranslationY = tempY;

            lastMainTotalX = e.TotalX;
            lastMainTotalY = e.TotalY;
        }
        else if (e.StatusType == GestureStatus.Completed)
        {
            IsMainGestureRunning = false;

            lastMainTotalX = 0.0;
            lastMainTotalY = 0.0;
        }
        else if (e.StatusType == GestureStatus.Canceled)
        {
            IsMainGestureRunning = false;

            lastMainTotalX = 0.0;
            lastMainTotalY = 0.0;
        }

    }

    private void HorizontalPanGestureRecognizer_PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        Debug.WriteLine($"HorizontalPanGesture - {e.StatusType}");

        if (e.StatusType == GestureStatus.Started)
        {
            IsHorizontalGestureRunning = false;

            lastHorizontalTotalX = 0.0;
        }
        else if (e.StatusType == GestureStatus.Running)
        {
            IsHorizontalGestureRunning = true;

            var deltaX = e.TotalX - lastHorizontalTotalX;
            
            var tempX = TranslationX + deltaX;

            // Keep it within min bounds
            if (tempX < 0)
            {
                tempX = 0;
            }


            // Keep it within max bounds
            if (Parent is Grid parentGrid)
            {
                if (tempX + Width > parentGrid.Width)
                {
                    tempX = parentGrid.Width - Width;
                }
            }

            TranslationX = tempX;

            lastHorizontalTotalX = e.TotalX;
        }
        else if (e.StatusType == GestureStatus.Completed)
        {
            IsHorizontalGestureRunning = false;

            lastHorizontalTotalX = 0.0;
        }
        else if (e.StatusType == GestureStatus.Canceled)
        {
            IsHorizontalGestureRunning = false;

            lastHorizontalTotalX = 0.0;
        }
    }


    private void VerticalPanGestureRecognizer_PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        Debug.WriteLine($"VerticalPanGesture - {e.StatusType}");

        if (e.StatusType == GestureStatus.Started)
        {
            IsVerticalGestureRunning = false;

            lastVerticalTotalY = 0.0;
        }
        else if (e.StatusType == GestureStatus.Running)
        {
            IsVerticalGestureRunning = true;

            var deltaY = e.TotalY - lastVerticalTotalY;

            var tempY = TranslationY + deltaY;

            // Keep it within min bounds
            if (tempY < 0)
            {
                tempY = 0;
            }

            // Keep it within max bounds
            if (Parent is Grid parentGrid)
            {
                if (tempY + Height > parentGrid.Height)
                {
                    tempY = parentGrid.Height - Height;
                }
            }
            TranslationY = tempY;

            lastVerticalTotalY = e.TotalY;
        }
        else if (e.StatusType == GestureStatus.Completed)
        {
            IsVerticalGestureRunning = false;

            lastVerticalTotalY = 0.0;
        }
        else if (e.StatusType == GestureStatus.Canceled)
        {
            IsVerticalGestureRunning = false;

            lastVerticalTotalY = 0.0;
        }

    }
}
