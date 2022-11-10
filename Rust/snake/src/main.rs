extern crate glutin_window;
extern crate graphics;
extern crate opengl_graphics;
extern crate piston;
extern crate find_folder;

use piston::window::WindowSettings;
use piston::event_loop::*;
use piston::input::*;
use glutin_window::GlutinWindow;
use opengl_graphics::{GlGraphics, OpenGL};

//use rand::{thread_rng, Rng};
use rand::Rng;

use std::collections::LinkedList;

use opengl_graphics::*;
use opengl_graphics::GlyphCache;


#[derive(Clone, PartialEq)]
enum Direction
{
    Right,
    Left,
    Up,
    Down
}


#[derive(Clone, PartialEq)]
struct Segment(u32, u32);


struct Snake
{
    body: LinkedList<Segment>,
    dir: Direction,
    sq_w: u32,
}

impl Snake
{
    fn new(x: u32, y: u32 , w: u32) -> Self
    {
        let mut list: LinkedList<Segment> = LinkedList::new();
        list.push_back(Segment(x, y));
        list.push_back(Segment(x-1, y));
        list.push_back(Segment(x-2, y));
        list.push_back(Segment(x-3, y));
        Snake
        {
            body: list,
            dir: Direction::Right,
            sq_w: w,
        }
    }

    fn render(&self, gl: &mut GlGraphics, args: &RenderArgs, game_over : bool)
    {
        const RED: [f32; 4] = [1.0, 0.0, 0.0, 1.0];
        const HEAD: [f32; 4] = [1.0, 1.0, 0.0, 1.0];
        let mut squares: Vec<graphics::types::Rectangle> = 
            self.body.iter()
                .map(|p| graphics::rectangle::square((p.0*self.sq_w) as f64, (p.1*self.sq_w) as f64, self.sq_w as f64))
                .collect();
        gl.draw(args.viewport(), |c, gl|
            {
                let transform = c.transform;
                graphics::rectangle(HEAD, squares.remove(0), transform, gl);
                squares.into_iter().for_each(|square| graphics::rectangle(RED, square, transform, gl));
            });
        if game_over
        {
            let mut glyph_cache = GlyphCache::new("assets/FiraSans-Regular.ttf", (), TextureSettings::new()).unwrap();
            gl.draw(args.viewport(), |c, gl|
                {
                    use graphics::*;
                    text::Text::new_color([0.0, 0.5, 0.0, 1.0], 32).draw("game over... (press space)",
                                                                     &mut glyph_cache,
                                                                     &DrawState::default(),
                                                                     c.transform.trans(10.0, 100.0),
                                                                     gl).unwrap();
                });
        }
    }

    fn update(&mut self, rows: u32, cols: u32, food: &mut Option<Segment>) -> bool
    {
        let old_head = self.body.front().expect("No head");
        if
               self.dir == Direction::Up    && old_head.1 == 0
            || self.dir == Direction::Down  && old_head.1 == rows-1
            || self.dir == Direction::Left  && old_head.0 == 0
            || self.dir == Direction::Right && old_head.0 == cols-1
        {
            return false;
        }
        let mut head = old_head.clone();
        match self.dir
        {
            Direction::Left  => head.0 -= 1,
            Direction::Right => head.0 += 1,
            Direction::Up    => head.1 -= 1,
            Direction::Down  => head.1 += 1,
        }
        if self.body.contains(&head)
        {
            return false;
        }

        if food.as_ref().unwrap() == &head
        {
            food.take();
        }
        else
        {
            self.body.pop_back();
        }

        self.body.push_front(head);

        return true;
    }

    fn pressed(&mut self, btn: &Button)
    {
        self.dir = match btn
        {
            &Button::Keyboard(Key::Up)    | &Button::Keyboard(Key::W) if self.dir != Direction::Down  => Direction::Up,
            &Button::Keyboard(Key::Down)  | &Button::Keyboard(Key::S) if self.dir != Direction::Up    => Direction::Down,
            &Button::Keyboard(Key::Left)  | &Button::Keyboard(Key::A) if self.dir != Direction::Right => Direction::Left,
            &Button::Keyboard(Key::Right) | &Button::Keyboard(Key::D) if self.dir != Direction::Left  => Direction::Right,

            _ => self.dir.clone(),
        };
    }
}


struct Game
{
    gl: GlGraphics,
    rows: u32,
    cols: u32,
    snake: Snake,
    food: Option<Segment>,
    sq_w: u32,
}

impl Game
{
    fn place_food(&mut self)
    {
        loop
        {
            let mut rng = rand::thread_rng();
            let random_x = rng.gen_range(0..self.cols);
            let random_y = rng.gen_range(0..self.rows);

            let seg = Segment(random_x, random_y);
            if !self.snake.body.contains(&seg)
            {
                self.food = Some(seg);
                break;
            }
        }
    }
    
    fn render(&mut self, arg: &RenderArgs, game_over : bool)
    {
        const GREEN: [f32; 4] = [0.0, 1.0, 0.0, 1.0];
        self.gl.draw(arg.viewport(), |_c, gl| 
            {
                graphics::clear(GREEN, gl);
            });
        self.snake.render(&mut self.gl, arg, game_over);

        if self.food.is_some()
        {
            const BLUE: [f32; 4] = [0.0, 0.0, 1.0, 1.0];
            let f = self.food.as_ref().unwrap();
            let square = 
                graphics::rectangle::square((f.0*self.sq_w) as f64, (f.1*self.sq_w) as f64, self.sq_w as f64);
            self.gl.draw(arg.viewport(), |c, gl|
                {
                    let transform = c.transform;
                    graphics::rectangle(BLUE, square, transform, gl);
                });

            }
        }
    
    fn update(&mut self) -> bool
    {
        if self.food.is_none()
        {
            self.place_food();
        }
        return self.snake.update(self.rows, self.cols, &mut self.food);
    }

    fn pressed(&mut self, btn: &Button) -> bool
    {
        let b = match btn
        {
            &Button::Keyboard(Key::Space) => true,
            _ => false,
        };
        if !b
        {
            self.snake.pressed(&btn);
        }
        return b;
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

    loop
    {
        let mut game = Game
        {
            gl: GlGraphics::new(opengl),
            rows: ROWS,
            cols: COLS,
            snake: Snake::new(COLS/2, ROWS/2, SQ_W),
            food: None,
            sq_w: SQ_W,
        };

        let mut events = Events::new(EventSettings::new()).ups(5);

        let mut but: Option<Button> = None;

        let mut ok : bool = true;

        while let Some(e) = events.next(&mut window)
        {
            if let Some(r) = e.render_args()
            {
                game.render(&r, !ok);
            }
            if let Some(_u) = e.update_args()
            {
                if but.is_some()
                {
                    if game.pressed(&but.unwrap())
                    {
                        break;
                    }
                    but = None;
                }
                if ok
                {
                    ok = game.update();//&u);
                }
            }
            if let Some(k) = e.button_args()
            {
                if k.state == ButtonState::Press
                {
                    if but.is_none()
                    {
                        but = Some(k.button.clone());
                    }
                }
            }
        }
    }
}
