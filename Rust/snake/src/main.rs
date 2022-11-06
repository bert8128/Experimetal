extern crate glutin_window;
extern crate graphics;
extern crate opengl_graphics;
extern crate piston;

use piston::window::WindowSettings;
use piston::event_loop::*;
use piston::input::*;
use glutin_window::GlutinWindow;
use opengl_graphics::{GlGraphics, OpenGL};

use std::collections::LinkedList;
use std::iter::FromIterator;

#[derive(Clone, PartialEq)]
enum Direction
{
    Right,
    Left,
    Up,
    Down
}

#[derive(Clone)]
struct Segment(u32, u32);
struct Body
{
    body: LinkedList<Segment>,
}
impl Default for Body {
    fn default() -> Self
    {
        Self { value: 0 }
    }
}

pub struct Snake
{
    body: Body,
    dir: Direction,
    sq_w: u32,
}
impl Snake
{
    fn render(&self, gl: &mut GlGraphics, args: &RenderArgs)
    {
        const RED: [f32; 4] = [1.0, 0.0, 0.0, 1.0];
        let squares: Vec<graphics::types::Rectangle> = 
            self.body.body
                .iter()
                .map(|p| Segment(p.0*self.sq_w, p.1*self.sq_w))
                .map(|p| graphics::rectangle::square(p.0 as f64, p.1 as f64, self.sq_w as f64))
                .collect();
        gl.draw(args.viewport(), |c, gl|
            {
                let transform = c.transform;
                squares
                    .into_iter()
                    .for_each(|square| graphics::rectangle(RED, square, transform, gl));
            });
    }
    pub fn update(&mut self, rows: u32, cols: u32)
    {
        let  old_head = self.body.front().expect("No head");
        println!("{} {}", old_head.0.to_string(), old_head.1.to_string());
        if
               self.dir == Direction::Up    && old_head.1 == 0
            || self.dir == Direction::Down  && old_head.1 == rows-1
            || self.dir == Direction::Left  && old_head.0 == 0
            || self.dir == Direction::Right && old_head.0 == cols-1
        {
            return;
        }
        let mut head = old_head.clone();
        match self.dir
        {
            Direction::Left  => head.0 -= 1,
            Direction::Right => head.0 += 1,
            Direction::Up    => head.1 -= 1,
            Direction::Down  => head.1 += 1,
        }
        self.body.push_front(head);
        self.body.pop_back();
    }

    fn pressed(&mut self, btn: &Button)
    {
        self.dir = match btn
        {
            &Button::Keyboard(Key::Up)    if self.dir != Direction::Down  => Direction::Up,
            &Button::Keyboard(Key::Down)  if self.dir != Direction::Up    => Direction::Down,
            &Button::Keyboard(Key::Left)  if self.dir != Direction::Right => Direction::Left,
            &Button::Keyboard(Key::Right) if self.dir != Direction::Left  => Direction::Right,

            &Button::Keyboard(Key::W)  if self.dir != Direction::Down  => Direction::Up,
            &Button::Keyboard(Key::S)  if self.dir != Direction::Up    => Direction::Down,
            &Button::Keyboard(Key::A)  if self.dir != Direction::Right => Direction::Left,
            &Button::Keyboard(Key::D)  if self.dir != Direction::Left  => Direction::Right,

            _ => self.dir.clone(),
        };
    }
}

pub struct Game
{
    gl: GlGraphics,
    rows: u32,
    cols: u32,
    snake: Snake,
}
impl Game
{
    fn render(&mut self, arg: &RenderArgs)
    {
        const GREEN: [f32; 4] = [0.0, 1.0, 0.0, 1.0];
        self.gl.draw(arg.viewport(), |_c, gl| 
            {
                graphics::clear(GREEN, gl);
            });
        self.snake.render(&mut self.gl, arg);
    }
    
    fn update(&mut self)
    {
        self.snake.update(self.rows, self.cols);
    }

    fn pressed(&mut self, btn: &Button)
    {
        self.snake.pressed(&btn);
    }
}

fn main()
{
    let opengl = OpenGL::V3_2;

    const COLS: u32 = 30;
    const ROWS: u32 = 20;
    const SQ_W: u32 = 20;

    const WIDTH:  u32 = COLS * SQ_W;
    const HEIGHT: u32 = ROWS * SQ_W;
    let mut window: GlutinWindow = WindowSettings::new("Snake", [WIDTH,HEIGHT]).graphics_api(opengl).exit_on_esc(true).build().unwrap();

    let mut game = Game
    {
        gl: GlGraphics::new(opengl),
        rows: ROWS,
        cols: COLS,
        snake: Snake
        {
            body: LinkedList::from_iter((vec![Segment(COLS/2,ROWS/2)]).into_iter()),
            dir: Direction::Right,
            sq_w: SQ_W
        },
    };

    let mut events = Events::new(EventSettings::new()).ups(5);

    while let Some(e) = events.next(&mut window)
    {
        if let Some(r) = e.render_args()
        {
            game.render(&r);
        }
        if let Some(u) = e.update_args()
        {
            game.update();//&u);
        }
        if let Some(k) = e.button_args()
        {
            if k.state == ButtonState::Press
            {
                game.pressed(&k.button);
            }
        }
    }
}
